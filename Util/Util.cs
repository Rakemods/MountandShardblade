using System;
using TaleWorlds.InputSystem;

namespace MountandShardblade.Util
{
    public static class Util  // Fixed: Renamed class to InputUtil to avoid duplicate name issue
    {
        public static bool AssignInputKey(string str, out InputKey key)
        {
            if (str.Equals("x1mousebutton", StringComparison.OrdinalIgnoreCase))
            {
                key = InputKey.X1MouseButton;
            }
            else if (str.Equals("x2mousebutton", StringComparison.OrdinalIgnoreCase))
            {
                key = InputKey.X2MouseButton;
            }
            else
            {
                key = InputKey.Tilde;
                return false;
            }

            return true;
        }

        public static bool AssignInputKey(char c, out InputKey key)
        {
            switch (char.ToLower(c))
            {
                case 'z':
                    key = InputKey.Z;
                    break;
                case 'x':
                    key = InputKey.X;
                    break;
                case 'c':
                    key = InputKey.C;
                    break;
                case 'v':
                    key = InputKey.V;
                    break;
                case 'b':
                    key = InputKey.B;
                    break;
                default:
                    key = InputKey.Tilde;
                    return false;
            }

            return true;
        }
    }
}
