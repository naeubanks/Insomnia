using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;


namespace Insomnia
{
    public class PowerPlan
    {
        public readonly string name;
        public Guid guid;

        public PowerPlan(Guid guid)
        {
            this.name = PowerHelpers.GetPowerPlanFriendlyName(guid);
            this.guid = guid;
        }

        public override bool Equals(object other)
        {
            PowerPlan otherPlan = (PowerPlan)other;
            if (otherPlan == null)
            {
                return false;
            }

            return otherPlan.guid.Equals(this.guid);
        }

    }

    class PowerHelpers
    {
        public static void SetActive(PowerPlan plan)
        {
            PowerSetActiveScheme(IntPtr.Zero, ref plan.guid);
        }

        public static List<PowerPlan> GetPlans()
        {
            List<PowerPlan> plans = new List<PowerPlan>();
            List<Guid> planGuids = new List<Guid>();

            IntPtr ReadBuffer;
            uint BufferSize = 16;

            uint Index = 0;
            uint ReturnCode = 0;

            while (ReturnCode == 0)
            {
                ReadBuffer = Marshal.AllocHGlobal((int)BufferSize);

                try
                {
                    ReturnCode = PowerEnumerate(IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, PowerDataAccessor.ACCESS_SCHEME, Index, ReadBuffer, ref BufferSize);

                    if (ReturnCode == 259) break; // no more data
                    if (ReturnCode != 0)
                    {
                        throw new COMException("Error occurred while enumerating power schemes. Win32 error code: " + ReturnCode);
                    }

                    Guid NewGuid = (Guid)Marshal.PtrToStructure(ReadBuffer, typeof(Guid));
                    planGuids.Add(NewGuid);
                }
                finally
                {
                    Marshal.FreeHGlobal(ReadBuffer);
                }

                Index++;
            }

            planGuids.ForEach(delegate (Guid guid)
            {
                plans.Add(new PowerPlan(guid));
            });

            return plans;
        }

        private static Guid GetActiveGuid()
        {
            Guid activePlan = Guid.Empty;
            IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(IntPtr)));
            if (PowerGetActiveScheme((IntPtr)null, out ptr) == 0)
            {
                activePlan = (Guid)Marshal.PtrToStructure(ptr, typeof(Guid));
                if (ptr != null)
                {
                    Marshal.FreeHGlobal(ptr);
                }
            }
            return activePlan;
        }

        public static PowerPlan GetCurrentPlan()
        {
            Guid guid = GetActiveGuid();
            return GetPlans().Find(p => (p.guid == guid));
        }

        public static string GetPowerPlanFriendlyName(Guid guid)
        {

            uint buffSize = 0;
            uint res = PowerReadFriendlyName(IntPtr.Zero, ref guid, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, ref buffSize);
            if (res == 0)
            {
                if (buffSize == 0u) return "";
                IntPtr ptrName = Marshal.AllocHGlobal((int)buffSize);
                res = PowerReadFriendlyName(IntPtr.Zero, ref guid, IntPtr.Zero, IntPtr.Zero, ptrName, ref buffSize);
                if (res == 0)
                {
                    string ret = Marshal.PtrToStringUni(ptrName);
                    Marshal.FreeHGlobal(ptrName);
                    return ret;
                }
                Marshal.FreeHGlobal(ptrName);
            }

            return String.Empty;
        }

        public static void TurnOffMonitor()
        {
            PostMessage(new IntPtr(HWND_TOPMOST), WM_SYSCOMMAND, new IntPtr(SC_MONITORPOWER), new IntPtr(MONITOR_OFF));
        }

        [DllImport("powrprof.dll", SetLastError = true)]
        static extern uint PowerEnumerate(IntPtr RootPowerKey,
                      IntPtr SchemeGuid,
                      IntPtr SubGroupOfPowerSettingsGuid,
                      PowerDataAccessor AccessFlags,
                      uint Index,
                      IntPtr Buffer,
                      ref uint BufferSize);

        enum PowerDataAccessor : uint
        {
            ACCESS_AC_POWER_SETTING_INDEX = 0,
            ACCESS_DC_POWER_SETTING_INDEX = 1,
            ACCESS_SCHEME = 16,
            ACCESS_SUBGROUP = 17,
            ACCESS_INDIVIDUAL_SETTING = 18,
            ACCESS_ACTIVE_SCHEME = 19,
            ACCESS_CREATE_SCHEME = 20
        }

        [DllImportAttribute("powrprof.dll", EntryPoint = "PowerSetActiveScheme")]
        public static extern uint PowerSetActiveScheme(IntPtr UserPowerKey, ref Guid ActivePolicyGuid);

        [DllImportAttribute("powrprof.dll", EntryPoint = "PowerGetActiveScheme")]
        public static extern uint PowerGetActiveScheme(IntPtr UserPowerKey, out IntPtr ActivePolicyGuid);

        [DllImportAttribute("powrprof.dll", EntryPoint = "PowerReadFriendlyName")]
        public static extern uint PowerReadFriendlyName(IntPtr RootPowerKey, ref Guid SchemeGuid, IntPtr SubGroupOfPowerSettingsGuid, IntPtr PowerSettingGuid, IntPtr Buffer, ref uint BufferSize);

        [DllImport("user32.dll")]
        static extern IntPtr PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        static readonly int HWND_TOPMOST = -1;
        static readonly int SC_MONITORPOWER = 0xF170;
        static readonly uint WM_SYSCOMMAND = 0x0112;
        static readonly int MONITOR_OFF = 2;
    }
}

