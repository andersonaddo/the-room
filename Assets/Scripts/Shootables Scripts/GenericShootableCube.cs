using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GenericShootableCube : MonoBehaviour, IShootableCube, IFreezable, ISelfDestructInstructions
{

    public List<cubeConfig> possibleConfigurations = new List<cubeConfig>();

    [SerializeField] cubeTypes _type;
    public cubeTypes type
    {
        get { return _type; }
    }
    
    bool _isFrozen;
    public bool isFrozen
    {
        get { return _isFrozen; }
        set { _isFrozen = value; }
    }

    [SerializeField] float rotSpeed, explosionRadius, explosionForce;
    Vector3 prefreezeAngularVelocity, prefreezeLinearVelocity;
    float instanciationVeclocityMagnitude;

    public int bulletShotScore, megaCubeScore;
    protected Rigidbody rb;

    protected bool isBeingAttracted;
    protected Transform attractingAbsorber;
    public float baseAttractionSpeed;

    public void initialize()
    {
        GetComponent<BoxCollider>().enabled = true;
        rb = GetComponent<Rigidbody>();
        instanciationVeclocityMagnitude = rb.velocity.magnitude;
        determineIfShouldBeAttracted(roomVisualsChanger.currentTheme);
        roomVisualsChanger.themeChanged += determineIfShouldBeAttracted;
        rb.angularVelocity = Random.insideUnitSphere * rotSpeed;
        GetComponent<timedSelfDestruct>().startTimer();
        GetComponent<iTweenStartScaler>().scale();
    }

    public void useConfig(cubeTypes configType)
    {
        cubeConfig config = possibleConfigurations.Where(c => c.type == configType).FirstOrDefault();
        GetComponent<Light>().color = config.lightColor;
        GetComponent<MeshRenderer>().material = config.material;
        _type = config.type;
    }

    virtual protected void resetForPooling()
    {
        isBeingAttracted = false;
        attractingAbsorber = null;
        isFrozen = false;
        rb.isKinematic = false;
        GetComponent<timedSelfDestruct>().cancel();
        roomVisualsChanger.themeChanged -= determineIfShouldBeAttracted;
        GetComponent<BoxCollider>().enabled = false;
        objectPooler.Instance.returnObject("genericCube", gameObject);
    }

    public void selfDestruct()
    {
        resetForPooling();
    }

    void FixedUpdate()
    {
        if (isBeingAttracted && !isFrozen)
        {
            rb.MovePosition(Vector3.MoveTowards(transform.position, 
                attractingAbsorber.position,
                baseAttractionSpeed * Time.deltaTime * difficultyCurveHolder.getCurrentValue(difficultyCurveHolder.Instance.absorberStrengthMultiplier)));
            Debug.DrawLine(transform.position, attractingAbsorber.position, Color.red);
        }
    }

    public void determineIfShouldBeAttracted(roomVisualsHolder holder)
    {
        if (holder.targetType == type)
        {
            attractingAbsorber = gameManager.Instance.absorbers.OrderBy(absorber => Vector3.SqrMagnitude(transform.position - absorber.transform.position)).First();
            rb.velocity = Vector3.zero;
            isBeingAttracted = true;
        }
        else if (isBeingAttracted) //Cube was being attracted before but not anymore...
        {
            isBeingAttracted = false;
            if (isFrozen) prefreezeLinearVelocity = Random.onUnitSphere * instanciationVeclocityMagnitude;
            else rb.velocity = Random.onUnitSphere * instanciationVeclocityMagnitude;
        }
    }

    virtual public void onShot(Vector3 position, damageEffectors effector)
    {
        calculatePoints(effector);
        explode(position);
    }

    public void freeze()
    {
        isFrozen = true;
        prefreezeAngularVelocity = rb.angularVelocity;
        prefreezeLinearVelocity = rb.velocity;
        GetComponent<timedSelfDestruct>().cancel();
        rb.isKinematic = true;
    }

    public void unfreeze(bool destroyIfPossible, damageEffectors effector)
    {
        isFrozen = false;
        if (destroyIfPossible)
        {
            calculatePoints(effector);
            explode(transform.position);
        }
        else
        {
            rb.isKinematic = false;
            rb.angularVelocity = prefreezeAngularVelocity;
            if (prefreezeLinearVelocity == Vector3.zero) rb.velocity = Random.onUnitSphere* instanciationVeclocityMagnitude; //Meaning it was being attracted to an absorber whenit was frozen...
            else rb.velocity = prefreezeLinearVelocity;
        }
    }

    void explode(Vector3 position)
    {
        GameObject bc = objectPooler.Instance.requestObject("brokencube");
        bc.transform.position = transform.position;
        bc.transform.rotation = transform.rotation;
        bc.GetComponent<brokenCube>().initialize(
            GetComponent<Light>().color,
            GetComponent<MeshRenderer>().material,
            position,
            explosionForce,
            explosionRadius);
        resetForPooling();
    }

    protected void calculatePoints(damageEffectors effector)
    {
        if (effector == damageEffectors.bullet)
        {
            if (gameManager.Instance.currentTargetType == type || type == cubeTypes.special)
                gameManager.Instance.incrementScore(bulletShotScore);
        }
        else if (effector == damageEffectors.megaCube)
        {
            gameManager.Instance.incrementScore(bulletShotScore);
        }
    }

    void OnDestroy()
    {
        roomVisualsChanger.themeChanged -= determineIfShouldBeAttracted;
    }
}

[System.Serializable]
public class cubeConfig
{
    public cubeTypes type;
    public Color lightColor;
    public Material material;
}
