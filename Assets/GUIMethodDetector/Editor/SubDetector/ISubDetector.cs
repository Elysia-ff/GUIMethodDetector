using System.Collections.Generic;
using UnityEngine;

namespace GUIMethodDetector
{
    interface ISubDetector
    {
        List<MethodData> Run(Component component);
    }
}
