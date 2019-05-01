using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine
{
    class Protocol
    {
        public const int REQUEST_ENTER = 1;
        public const int RESPONSE_ENTER = 2;
        public const int BROADCAST_ENTER = 3;
        public const int REQUEST_CHANGE_DESTINATION = 4;
        public const int BROADCAST_CHANGE_DESTINATION = 5;
        public const int BROADCAST_EXIT = 6;
    }
}
