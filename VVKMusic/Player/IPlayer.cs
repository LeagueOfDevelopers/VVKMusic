﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Status = Common.Common.Status;

namespace Player
{
    interface IPlayer
    {
        Status SetSource(Uri source, bool downloaded);
        Status Play();
        Status Stop();
    }
}
