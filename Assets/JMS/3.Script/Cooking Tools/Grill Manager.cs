using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Cooking
{
    public class GrillManager : CookerManager
    {
        protected override void Awake()
        {
            base.Awake();

            Assert.IsNotNull(cookArea, $"[{gameObject.name}] Can't find collider for grill area");
        }

        protected override void Start()
        {
            base.Start();
        }
    }
}
