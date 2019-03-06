using UnityEngine;

public interface IShootableCube {

    cubeTypes _type { get; }
    void onShot(Vector3 shotPosition, damageEffectors damageEffector);
}

public enum cubeTypes {
    red,
    blue,
    green,
    purple,
    orange,
    special
}

public enum damageEffectors
{
    bullet,
    megaCube
}
