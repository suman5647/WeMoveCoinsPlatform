using System;
using System.Threading;
using System.Threading.Tasks;

namespace WMC.Utilities
{
    public class Retrier<TResult>
    {
        public TResult Try(Func<TResult> func, int maxRetries)
        {
            TResult returnValue = default(TResult);
            int numTries = 0;
            bool succeeded = false;
            while (numTries < maxRetries)
            {
                try {
                    returnValue = func();
                    succeeded = true;
                }
                catch (Exception)
                {
                    //todo: figure out what to do here   
                }
                finally { numTries++; }
                if (succeeded) return returnValue;
            }
            return default(TResult);
        }

       

        public TResult Try(Func<TResult> func, int maxRetries, int delayInMilliseconds)
        {
            TResult returnValue = default(TResult);
            int numTries = 0;
            bool succeeded = false;
            while (numTries < maxRetries)
            {
                try {
                    returnValue = func();
                    succeeded = true;
                }
                catch (Exception ex)
                {
                    //todo: figure out what to do here 
                }
                finally { numTries++; }
                if (succeeded)
                    return returnValue;
                Thread.Sleep(delayInMilliseconds);
            }
            return default(TResult);
        }

        //var service = new WebService();
        ////the type parameter is for the return type of
        ////the method we are retrying (GetDataFromRemoteServer()
        ////returns a string in this example)
        //var retrier = new Retrier<string>();
        ////call the service up to 3 times in the event of failure
        //string result = retrier.Try(() => service.GetDataFromRemoteServer(), 3);
        ////call the service up to 3 times,         ////wait 500ms if there is a failure

        //string result2 = retrier.TryWithDelay(() => service.GetDataFromRemoteServer(), 3, 500);

        public async Task<TResult> TryAsync(Func<Task<TResult>> func, int maxRetries, int delayInMilliseconds)
        {
            TResult returnValue = default(TResult);
            int numTries = 0;
            bool succeeded = false;
            while (numTries < maxRetries)
            {
                try
                {
                    returnValue = await func();
                    succeeded = true;
                }
                catch (Exception ex)
                {
                    //todo: figure out what to do here 
                }
                finally { numTries++; }
                if (succeeded)
                    return returnValue;
                Thread.Sleep(delayInMilliseconds);
            }
            return default(TResult);
        }
    }

}