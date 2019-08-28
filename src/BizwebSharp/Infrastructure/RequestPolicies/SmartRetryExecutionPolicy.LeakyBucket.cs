using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BizwebSharp.Infrastructure
{
    public partial class SmartRetryExecutionPolicy
    {
        private class LeakyBucket
        {
            private const int MAX_LIFE_SPAN = 1800; // = 30 minutes

            private const int DEFAULT_BUCKET_CAPACITY = 40;

            private int _capacity = DEFAULT_BUCKET_CAPACITY;

            private int _leakRate = 2;

            private static readonly ConcurrentDictionary<string, LeakyBucket> _shopAccessTokenToLeakyBucket =
                new ConcurrentDictionary<string, LeakyBucket>();

            private static Timer _dripAllBucketsTimer
                = new Timer(_ => DripAllBuckets(), null, THROTTLE_DELAY, THROTTLE_DELAY);

            private static Timer _tryCleanAllBucketsTimer
                = new Timer(_ => TryCleanAllBuckets(), null, MAX_LIFE_SPAN * 1000, 300000);

            private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(DEFAULT_BUCKET_CAPACITY, 1000);

            private DateTimeOffset LastUsedAt { get; set; }

            private LeakyBucket() { }

            public static LeakyBucket GetBucketByToken(string token)
            {
                return _shopAccessTokenToLeakyBucket.GetOrAdd(token, _ => new LeakyBucket());
            }

            public Task GrantAsync()
            {
                LastUsedAt = DateTimeOffset.UtcNow;
                return _semaphore.WaitAsync();
            }

            public void SetBucketState(int reportedFillLevel, int reportedCapacity)
            {
                LastUsedAt = DateTimeOffset.UtcNow;

                //Shopify Plus customers have a bucket that is twice the size (80) so we resize the bucket capacity accordingly
                //It is apparently possible to request the bucket size to be even larger
                //https://ecommerce.shopify.com/c/shopify-apis-and-technology/t/what-is-the-default-api-call-limit-on-shopify-stores-407292
                if (reportedCapacity != _capacity)
                {
                    lock (_semaphore)
                    {
                        //Because SetBucketState can be concurrently used, we should surround all set operations with lock.
                        //The following codes are in lock scope now, let's check the condition again.
                        if (reportedCapacity != _capacity)
                        {
                            if (reportedCapacity > _capacity)
                            {
                                _semaphore.Release(reportedCapacity - _capacity);
                            }

                            _capacity = reportedCapacity;

                            //Note that when the capacity doubles, the leak rate also doubles.
                            //So, not only can request bursts be larger, it is also possible to sustain a faster rate over the long term.
                            //This may not suit with bizweb.
                            //_leakRate = _capacity / DEFAULT_BUCKET_CAPACITY;
                        }
                    }
                }

                //Corrects the grant capacity of the bucket based on the size returned by Shopify.
                //Shopify may know that the remaining capacity is less than we think it is (for example if multiple programs are using that same token)
                //Shopify may also think that the remaining capacity is more than we know, but we do not ever empty the bucket because Shopify is not
                //considering requests that we know are already in flight.
                var grantCapacity = _capacity - reportedFillLevel;

                while (_semaphore.CurrentCount > grantCapacity)
                {
                    //We refill the virtual bucket accordingly.
                    _semaphore.Wait();
                }
            }

            private void AdaptiveDrip()
            {
                if (_semaphore.CurrentCount < _capacity)
                {
                    int freeSlots = _capacity - _semaphore.CurrentCount;
                    _semaphore.Release(Math.Min(_leakRate, freeSlots));
                }
            }

            private static void DripAllBuckets()
            {
                //Parallel.ForEach(_allLeakyBuckets, bucket => bucket.ReleaseOne());
                foreach (var bucket in _shopAccessTokenToLeakyBucket.Values)
                {
                    bucket.AdaptiveDrip();
                }
            }

            private bool IsEndOfLife()
            {
                return (DateTimeOffset.UtcNow - LastUsedAt).TotalSeconds >= MAX_LIFE_SPAN;
            }

            private static void TryCleanAllBuckets()
            {
                foreach (var bucketKVP in _shopAccessTokenToLeakyBucket.Where(s => s.Value.IsEndOfLife()))
                {
                    _shopAccessTokenToLeakyBucket.TryRemove(bucketKVP.Key, out var bucket);
                    bucket = null;
                }
            }
        }
    }
}
