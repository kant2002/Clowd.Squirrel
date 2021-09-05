using System;
using System.Collections;
using System.Runtime.InteropServices;

namespace Squirrel.Shell
{
    internal class ShellComWrappers : ComWrappers
    {
        private const int S_OK = (int)Interop.HRESULT.S_OK;
        private static readonly Guid IID_IStream = new Guid(0x0000000C, 0x0000, 0x0000, 0xC0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x46);

        private static readonly ComInterfaceEntry* s_wrapperEntry = InitializeComInterfaceEntry();

        internal static ShellComWrappers Instance { get; } = new ShellComWrappers();

        private ShellComWrappers() { }

        private static ComInterfaceEntry* InitializeComInterfaceEntry()
        {
            GetIUnknownImpl(out IntPtr fpQueryInteface, out IntPtr fpAddRef, out IntPtr fpRelease);

            IntPtr iStreamVtbl = IStreamVtbl.Create(fpQueryInteface, fpAddRef, fpRelease);

            ComInterfaceEntry* wrapperEntry = (ComInterfaceEntry*)RuntimeHelpers.AllocateTypeAssociatedMemory(typeof(DrawingComWrappers), sizeof(ComInterfaceEntry));
            wrapperEntry->IID = IID_IStream;
            wrapperEntry->Vtable = iStreamVtbl;
            return wrapperEntry;
        }

        protected override unsafe ComInterfaceEntry* ComputeVtables(object obj, CreateComInterfaceFlags flags, out int count)
        {
            // Always return the same table mappings.
            count = 1;
            return s_wrapperEntry;
        }

        protected override object CreateObject(IntPtr externalComObject, CreateObjectFlags flags)
        {
            Debug.Assert(flags == CreateObjectFlags.UniqueInstance);

            Guid pictureIID = IPicture.IID;
            int hr = Marshal.QueryInterface(externalComObject, ref pictureIID, out IntPtr comObject);
            if (hr == S_OK)
            {
                return new TrayNotifyWrapper(comObject);
            }

            throw new NotImplementedException();
        }

        protected override void ReleaseObjects(IEnumerable objects)
        {
            throw new NotImplementedException();
        }

    }
}
