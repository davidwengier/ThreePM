using System;
using System.Collections.Generic;
using System.Text;

namespace ThreePM.MusicPlayer
{
    public enum PlaylistStyle
    {
        Normal,
        Random,
        Looping,
        RandomLooping
    }

    public enum PlayerState
    {
        Stopped,
        Playing,
        Paused
    }
}
