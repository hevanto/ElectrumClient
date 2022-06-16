using System.Reflection;
using System.Resources;

namespace ElectrumClient
{
    internal static class Resources
    {
        private static bool _assemblyInfoLoaded = false;
        private static bool _resourcesFileLoaded = false;

        private static string _version = "";
        private static string _name = "";
        private static string _company = "";

        private static string _protocolMin = "";
        private static string _protocolMax = "";
        private static string _donationAddress = "";
        private static string _testnetHost = "";
        private static int _testnetPort;
        private static bool _testnetSSL;
        private static string _mainnetHost = "";
        private static int _mainnetPort;
        private static bool _mainnetSSL;


        public static string Version
        {
            get
            {
                GetAssemblyDetails();
                return _version;
            }
        }
        public static string Name
        {
            get
            {
                GetAssemblyDetails();
                return _name;
            }
        }

        public static string Company
        {
            get
            {
                GetAssemblyDetails();
                return _company;
            }
        }

        public static string ProtocolMin
        {
            get
            {
                GetResources();
                return _protocolMin;
            }
        }

        public static string ProtocolMax
        {
            get
            {
                GetResources();
                return _protocolMax;
            }
        }

        public static string DonationAddress
        {
            get
            {
                GetResources();
                return _donationAddress;
            }
        }

        public static string TestnetHost
        {
            get
            {
                GetResources();
                return _testnetHost;
            }
        }

        public static int TestnetPort
        {
            get
            {
                GetResources();
                return _testnetPort;
            }
        }

        public static bool TestnetSSL
        {
            get
            {
                GetResources();
                return _testnetSSL;
            }
        }

        public static string MainnetHost
        {
            get
            {
                GetResources();
                return _mainnetHost;
            }
        }

        public static int MainnetPort
        {
            get
            {
                GetResources();
                return _mainnetPort;
            }
        }

        public static bool MainnetSSL
        {
            get
            {
                GetResources();
                return _mainnetSSL;
            }
        }

        private static void GetAssemblyDetails()
        {
            if (_assemblyInfoLoaded) return;
            _assemblyInfoLoaded = true;

            var assembly = Assembly.GetAssembly(typeof(Client));
            if (assembly == null) return;

            AssemblyName nameObj = assembly.GetName();
            _name = nameObj.Name ?? "";
            if (nameObj.Version != null) _version = nameObj.Version.ToString();

            var companyAttrib = assembly.GetCustomAttribute<System.Reflection.AssemblyCompanyAttribute>();
            if (companyAttrib != null) _company = companyAttrib.Company;
        }

        private static void GetResources()
        {
            if (_resourcesFileLoaded) return;
            _resourcesFileLoaded = true;

            ResourceManager mgr = new ResourceManager(typeof(Properties.Resources));
            _protocolMin = mgr.GetString("ProtocolMin") ?? "1.4";
            _protocolMax = mgr.GetString("ProtocolMax") ?? "1.4";
            _donationAddress = mgr.GetString("DonationAddress") ?? "";
            _testnetHost = mgr.GetString("TestnetHost") ?? "";
            _testnetPort = Convert.ToInt32(mgr.GetString("TestnetPort") ?? "60001");
            _testnetSSL = Convert.ToBoolean(mgr.GetString("TestnetSSL") ?? "false");
            _mainnetHost = mgr.GetString("MainnetHost") ?? "";
            _mainnetPort = Convert.ToInt32(mgr.GetString("MainnetPort") ?? "50001");
            _mainnetSSL = Convert.ToBoolean(mgr.GetString("MainnetSSL") ?? "false");
        }
    }
}
