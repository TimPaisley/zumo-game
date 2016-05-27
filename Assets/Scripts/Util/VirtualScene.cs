using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/**
 * Since everything in the game (menus and gameplay) happens in one scene,
 * virtual scenes are used to separate the different game states
 */
public abstract class VirtualScene : MonoBehaviour {
    public virtual void Activate () {
        gameObject.SetActive(true);
    }

    public virtual void Deactivate () {
        gameObject.SetActive(false);
    }
}
