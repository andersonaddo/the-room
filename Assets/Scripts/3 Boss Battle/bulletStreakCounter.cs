using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletStreakCounter
{
    public static int bulletsLaunched {get; private set;}
    public static int consecutiveBulletsDestroyed {get; private set;}

    //Will store the bullet gameobject iDs instead of their
    static Queue<int> bulletQueue = new Queue<int>();  
 
    public static void signalBulletLaunch(GameObject bullet){
        bulletQueue.Enqueue(bullet.gameObject.GetInstanceID());
        bulletsLaunched++;
    }

    //This is potentially flawed because we pool these bullets, so this could 
    //Give a falso positive, but for now it's alright
    public static void signalBulletDestruction(GameObject bullet){
        if (bulletQueue.Peek() == bullet.GetInstanceID()){
            consecutiveBulletsDestroyed++;
            bulletQueue.Dequeue();
        }else{
            consecutiveBulletsDestroyed = 1;
            //Keep Dequeueing until you dequeue this bullet's id
            //Fine since Objectpooler pools sequentially and inearly
            int id = 0;
            while (id != bullet.GetInstanceID()){
                id = bulletQueue.Dequeue();
            }
        }
    }

    public static void reset(){
        consecutiveBulletsDestroyed = 0;
        bulletsLaunched = 0;
        bulletQueue.Clear();
    }
}
