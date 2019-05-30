using UnityEngine;

public interface IShootableCube {

    cubeTypes type { get; }
    void onShot(Vector3 shotPosition, damageEffectors damageEffector);
}

public enum cubeTypes {
    red,
    blue,
    green,
    special
}

public enum damageEffectors
{
    bullet,
    megaCube
}
