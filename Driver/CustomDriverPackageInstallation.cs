

//---------------------------------------------------------------------------
// Microsoft Customizable Driver Package installation Task
//---------------------------------------------------------------------------

using WEX.TestExecution.Markup;
using System;
using System.Data;
using System.Threading;
using WEX.Logging.Interop;
using WEX.TestExecution;
using Microsoft.WDTF;


namespace Microsoft.DriverKit
{
    //**************************************************
    //Main Driver Package installation class
    //**************************************************
    [TestClass]
    public class CustomDriverPackageInstallationClass
    {

        //*************************************************
        //  Class member variables
        //*************************************************

        static private String m_sAbsoluteDriverPackagePath;


        //*************************************************
        //  PerformDriverPackageInstallation
        //*************************************************

        [TestMethod]
        // Required properties (see Windows Driver Kit documentation for optional properties):
        [TestProperty("RebootPossible", "true")]
        [TestProperty("Kits.Drivers", "")]
        [TestProperty("Kits.Drivers.Installer", "")]
        [TestProperty("Description", "Driver")]
        [TestProperty("Kits.DisplayName", "Driver")]
        [TestProperty("Kits.Category", "Driver Installation")]
        [TestProperty("Kits.Drivers.ResultFile", "setupapi.dev.log")]
        [TestProperty("Kits.Drivers.ResultFile", "setupapi.app.log")]
        [TestProperty("Kits.Drivers.ResultFile", "TestTextLog.log")]

        public void Driver()
        {

            //
            // Get the Absolute Driver Package Path string.
            //

            if (m_testContext.Properties["AbsoluteDriverPackagePath"] == null)
            {

                //
                //  Required parameter not supplied. Log an error and quit.
                //

                Log.Error(" The Driver Package Installation was not given the path to the Driver Package");

                return;
            }
            else
            {

                //
                //  Save Absolute Driver Package Path 
                //
                //  As this test is initiated from the VS/WDK developer system but
                //  executed on the test system we need to expand any environment variable in
                //  the path also.
                //

                m_sAbsoluteDriverPackagePath =
                    Environment.ExpandEnvironmentVariables(m_testContext.Properties["AbsoluteDriverPackagePath"].ToString());


            }


            //
            //  Init WDTF
            //

            IWDTF2 WDTF = new WDTF2Class();
            IWDTFDriverSetupSystemAction2 SystemDriverSetup = (IWDTFDriverSetupSystemAction2)WDTF.SystemDepot.ThisSystem.GetInterface("DriverSetup");
            IWDTFSystemAction2 System = (IWDTFSystemAction2)WDTF.SystemDepot.ThisSystem.GetInterface("System");
            IWDTFTargets2 Devices;


            //
            //  Init Driver Package Class
            //

            WDTF.Log.StartTestCase("Init Driver Package");

            WDTFDriverPackage2 DriverPackage = new WDTFDriverPackage2Class();

            DriverPackage.SetPackageInfFileName(m_sAbsoluteDriverPackagePath);

            //
            //  See if we started because of a reboot 
            //

            if (System.IsRestarted)
            {

                //
                //  We have restarted lets see if the driver package installed on any device
                //  If not we have failed.
                //

                Devices = WDTF.DeviceDepot.Query(DriverPackage.GetQueryForDeviceUsingPackage());

                if (Devices.Count == 0)
                {
                    WDTF.Log.OutputError(" Failed to install the driver package on any device ");
                }

                //
                // Driver package installed onto device , verify it started
                //

                foreach (IWDTFTarget2 Device in Devices)
                {
                    if (Device.Eval("ProblemCode!=0"))
                    {
                        WDTF.Log.OutputError("Driver package installed onto device but driver failed to start");
                        WDTF.Log.OutputInfo("Device:     " + Device.GetValue("UniqueTargetName"));
                        WDTF.Log.OutputInfo("Status:     " + Device.GetValue("DeviceStatusString"));
                    }
                }

                SystemDriverSetup.SnapTriageLogs();
                WDTF.Log.EndTestCase();
                return;
            }

            //
            //  Output some INF info 
            //  

            WDTF.Log.OutputInfo(" INF Info");
            WDTF.Log.OutputInfo("   Class: " + DriverPackage.ClassName);
            WDTF.Log.OutputInfo("   Provider: " + DriverPackage.Provider);
            WDTF.Log.OutputInfo("   Version: " + DriverPackage.Version);
            WDTF.Log.OutputInfo("   Date: " + DriverPackage.Date);
            WDTF.Log.OutputInfo("   CatalogFile: " + DriverPackage.CatalogFile);
            WDTF.Log.OutputInfo("   HasCatalog: " + DriverPackage.HasCatalog.ToString());
            WDTF.Log.OutputInfo("   IsImported: " + DriverPackage.IsImported.ToString());
            WDTF.Log.OutputInfo("   DigitalSigner: " + DriverPackage.DigitalSigner);
            WDTF.Log.OutputInfo("   IsDigitalSigned: " + DriverPackage.IsDigitalSigned.ToString());
            WDTF.Log.OutputInfo("   IsDigitalSignerTrusted: " + DriverPackage.IsDigitalSignerTrusted.ToString());

            //
            //  Determine if the system has a device that the package can be 
            //  installed on to.
            //

            Devices = WDTF.DeviceDepot.Query(DriverPackage.GetQueryForDeviceThatCanUsePackage());

            if (Devices.Count == 0)
            {
                WDTF.Log.OutputError(" System has no device the driver package can  be installed onto.");
                return;
            }

            Devices = null;

            WDTF.Log.EndTestCase();

            //
            //  Import driver package
            //

            WDTF.Log.StartTestCase("Import  Driver Package");

            SystemDriverSetup.ClearTriageLogs();

            SystemDriverSetup.ImportDriverPackage(DriverPackage);

            WDTF.Log.EndTestCase();

            //
            //  Install driver package onto device
            //

            WDTF.Log.StartTestCase("Install driver package onto device");

            Devices = WDTF.DeviceDepot.Query(DriverPackage.GetQueryForDeviceThatCanUsePackage());


            IWDTFActions2 DeviceDriverSetupActions = DeviceDriverSetupActions = Devices.GetInterfaces("DriverSetup");
            bool bRebootRequired = false;

            foreach (IWDTFDriverSetupAction2 DeviceDriverSetup in DeviceDriverSetupActions)
            {

                if (DeviceDriverSetup.UpdateDriver(DriverPackage))
                    bRebootRequired = true;
            }


            if (bRebootRequired == false)
            {

                Devices = WDTF.DeviceDepot.Query(DriverPackage.GetQueryForDeviceUsingPackage());

                if (Devices.Count == 0)
                {
                    WDTF.Log.OutputError(" Failed to install driver package on any device ");
                }

                //
                // Driver package installed onto device , verify it started
                //

                foreach (IWDTFTarget2 Device in Devices)
                {
                    if (Device.Eval("ProblemCode!=0"))
                    {
                        WDTF.Log.OutputError("Driver package installed onto device but driver failed to start");
                        WDTF.Log.OutputInfo("Device:     " + Device.GetValue("UniqueTargetName"));
                        WDTF.Log.OutputInfo("Status:     " + Device.GetValue("DeviceStatusString"));
                    }
                }

                SystemDriverSetup.SnapTriageLogs();
                WDTF.Log.EndTestCase();
                return;

            }
            else
            {

                //
                //  Update driver indecated we need reboot
                //

                WDTF.Log.EndTestCase();
                System.RebootRestart();
                return;

            }

        }

        private TestContext m_testContext;

        public TestContext TestContext
        {
            get { return m_testContext; }
            set { m_testContext = value; }
        }

    }
}
