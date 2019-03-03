using UnityEngine;

public interface IShootableCube {

    cubeTypes _type { get; }
    void onShot(Vector3 shotPosition);
}

public enum cubeTypes {
    red,
    blue,
    green,
    purple,
    orange,
    special
}
