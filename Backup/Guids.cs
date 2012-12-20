// Guids.cs
// MUST match guids.h
using System;

namespace PeterProvost.VsKeyboardShortcutsDump
{
    static class GuidList
    {
        public const string guidVsKeyboardShortcutsDumpPkgString = "5933efec-981b-4b12-ace8-72a012aae2a9";
        public const string guidVsKeyboardShortcutsDumpCmdSetString = "e5d2e960-3668-4dac-9a9a-cda83fee8f0e";

        public static readonly Guid guidVsKeyboardShortcutsDumpCmdSet = new Guid(guidVsKeyboardShortcutsDumpCmdSetString);
    };
}