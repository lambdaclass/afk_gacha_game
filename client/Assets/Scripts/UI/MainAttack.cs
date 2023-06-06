using UnityEngine;

public class MainAttack : MoreMountains.TopDownEngine.CharacterAbility
{
    GameObject directionIndicator;
    GameObject HackShoot;
    GameObject HackShootFeedback;
    protected override void Initialization()
    {
        base.Initialization();
    }
    public override void ProcessAbility()
    {
        base.ProcessAbility();
    }
    public void HackShootExecute()
    {
        ShowDirectionIndicator(transform.position);
        LaserCollision(transform.position);
    }
    public void ShowDirectionIndicator(Vector3 position)
    {
        directionIndicator = Instantiate(Resources.Load("AttackDirection", typeof(GameObject))) as GameObject;
        directionIndicator.transform.parent = transform;
        directionIndicator.transform.position = transform.position;

        HackShoot = Instantiate(Resources.Load("HackShoot", typeof(GameObject))) as GameObject;
        HackShoot.transform.parent = transform;
        HackShoot.transform.position = position;
    }
    public void LaserCollision(Vector3 position)
    {
        Destroy(directionIndicator, 0.1f);
        Destroy(HackShoot, 0.1f);
        HackShootFeedback = Instantiate(Resources.Load("HackShootFeedback", typeof(GameObject))) as GameObject;
        Destroy(HackShootFeedback, 1f);
        HackShootFeedback.transform.parent = transform;
        HackShootFeedback.transform.position = position;
    }
    public void LaserDisappear()
    {
        Destroy(directionIndicator, 0.1f);
        Destroy(HackShoot, 0.1f);
    }
}
