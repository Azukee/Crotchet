using System;
using Crotchet.InputSimulator;

namespace Crotchet.Quaver.Chart.Models
{
    public enum Lane
    {
        Lane1 = 1,
        Lane2,
        Lane3,
        Lane4,
        Lane5,
        Lane6,
        Lane7
    }

    public static class LaneExtensions
    {
        public static VirtualKeyCode ToVirtualKeycode(this Lane lane)
        {
            switch (lane) {
                case Lane.Lane1:
                    return VirtualKeyCode.VK_1;
                case Lane.Lane2:
                    return VirtualKeyCode.VK_2;
                case Lane.Lane3:
                    return VirtualKeyCode.VK_3;
                case Lane.Lane4:
                    return VirtualKeyCode.VK_4;
                case Lane.Lane5:
                    return VirtualKeyCode.VK_5;
                case Lane.Lane6:
                    return VirtualKeyCode.VK_6;
                case Lane.Lane7:
                    return VirtualKeyCode.VK_7;
                default:
                    throw new ArgumentOutOfRangeException(nameof(lane), lane, null);
            }
        }
    }
}