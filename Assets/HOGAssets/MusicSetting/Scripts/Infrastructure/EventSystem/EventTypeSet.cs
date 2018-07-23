//*************************************************************************
//@header       EventTypeSet
//@abstract     Set EventType.
//@discussion   New EventType with number and name.Number must be different.
//@version      v1.0.0
//@author       Felix Zhang
//@copyright    Copyright (c) 2017 FFTAI Co.,Ltd.All rights reserved.
//**************************************************************************

namespace FZ.HiddenObjectGame
{
    public static class EventTypeSet
    {
        public static EventType NextMusic = new EventType(0, "NextMusic");

        public static EventType GameStart = new EventType(10, "GameStart");
        public static EventType TargetCreated = new EventType(11, "TargetCreated");
        //public static EventType GameOver = new EventType(12, "GameOver");
        public static EventType FinishGame = new EventType(13, "FinishGame");
        //public static EventType NextFlow = new EventType(14, "NextFlow");
    }
}
