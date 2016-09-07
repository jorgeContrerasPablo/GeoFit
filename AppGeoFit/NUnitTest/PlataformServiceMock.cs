using AppGeoFit.BusinessLayer.Managers.GameManager;
using AppGeoFit.BusinessLayer.Managers.PlayerManager;
using AppGeoFit.BusinessLayer.Managers.TeamManager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace NUnitTest
{
    public class PlatformServicesMock : IPlatformServices
    {
        void IPlatformServices.BeginInvokeOnMainThread(Action action)
        {
            throw new NotImplementedException();
        }
        Assembly[] IPlatformServices.GetAssemblies()
        {
            // Recuperamos el ensamblador que queremos para las pruebas.
            Assembly[] assembly = new Assembly[3];
            assembly[0] = Assembly.GetAssembly(typeof(PlayerManager));
            assembly[1] = Assembly.GetAssembly(typeof(TeamManager));
            assembly[2] = Assembly.GetAssembly(typeof(GameManager));

            return assembly;
        }
        Task<Stream> IPlatformServices.GetStreamAsync(Uri uri, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
        IIsolatedStorageFile IPlatformServices.GetUserStoreForApplication()
        {
            throw new NotImplementedException();
        }
        void IPlatformServices.OpenUriAction(Uri uri)
        {
            throw new NotImplementedException();
        }
        void IPlatformServices.StartTimer(TimeSpan interval, Func<bool> callback)
        {
            throw new NotImplementedException();
        }

        string IPlatformServices.GetMD5Hash(string input)
        {
            throw new NotImplementedException();
        }

        double IPlatformServices.GetNamedSize(NamedSize size, Type targetElementType, bool useOldSizes)
        {
            throw new NotImplementedException();
        }

        ITimer CreateTimer(Action<object> callback)
        {
            throw new NotImplementedException();
        }

        ITimer CreateTimer(Action<object> callback, object state, int dueTime, int period)
        {
            throw new NotImplementedException();
        }

        ITimer CreateTimer(Action<object> callback, object state, long dueTime, long period)
        {
            throw new NotImplementedException();
        }

        ITimer CreateTimer(Action<object> callback, object state, TimeSpan dueTime, TimeSpan period)
        {
            throw new NotImplementedException();
        }

        ITimer CreateTimer(Action<object> callback, object state, uint dueTime, uint period)
        {
            throw new NotImplementedException();
        }

        ITimer IPlatformServices.CreateTimer(Action<object> callback)
        {
            throw new NotImplementedException();
        }

        ITimer IPlatformServices.CreateTimer(Action<object> callback, object state, int dueTime, int period)
        {
            throw new NotImplementedException();
        }

        ITimer IPlatformServices.CreateTimer(Action<object> callback, object state, long dueTime, long period)
        {
            throw new NotImplementedException();
        }

        ITimer IPlatformServices.CreateTimer(Action<object> callback, object state, TimeSpan dueTime, TimeSpan period)
        {
            throw new NotImplementedException();
        }

        ITimer IPlatformServices.CreateTimer(Action<object> callback, object state, uint dueTime, uint period)
        {
            throw new NotImplementedException();
        }

        bool IPlatformServices.IsInvokeRequired
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}
