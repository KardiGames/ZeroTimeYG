using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movie : MonoBehaviour
{
    //Loading prepetaions
    List<CombatAction> combatLog = Status.combatLog;
    public GameObject bulletPrefab;
    [SerializeField] AudioClip shootSound;
    [SerializeField] AudioClip hitSound;
    [SerializeField] AudioClip moveSound;
    [SerializeField] AudioClip enemyShootSound;
    [SerializeField] AudioClip enemyHitSound;
    [SerializeField] AudioClip enemyMoveSound;
    private GameObject bullet;

    //Settings for the movie
    int divider = 100;
    int numenator = 0;
    Vector3 summator;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (Status.Current != "movie") return;
        CombatAction thisAction = combatLog[Status.MovieAct];

        if (thisAction.action == "move")
        {

            if (summator == Vector3.zero)
            {
                summator.x = (CoordArray.cArray[(combatLog[Status.MovieAct].place[0]), (combatLog[Status.MovieAct].place[1]), 0] - combatLog[Status.MovieAct].subject.transform.position.x) / divider;
                summator.y = (CoordArray.cArray[(combatLog[Status.MovieAct].place[0]), (combatLog[Status.MovieAct].place[1]), 1] - combatLog[Status.MovieAct].subject.transform.position.y) / divider;

                //TODO - edit crutch with choosing sound
                if (thisAction.subject.ai=="")
                    thisAction.subject.characterSound.clip = moveSound;
                else
                    thisAction.subject.characterSound.clip = enemyMoveSound;

                thisAction.subject.characterSound.Play();

                if (thisAction.subject.characterAnimator != null)
                {
                    thisAction.subject.characterAnimator.SetBool("Run", true);
                      TurnAnimatedObject(thisAction.subject, summator.x);
                }

            }

            combatLog[Status.MovieAct].subject.transform.position += summator;

            if (++numenator == divider)
            {
                if (thisAction.subject.characterAnimator != null && !(Status.MovieAct < (combatLog.Count - 1) && combatLog[Status.MovieAct + 1].subject == thisAction.subject && combatLog[Status.MovieAct + 1].action == "move"))
                    thisAction.subject.characterAnimator.SetBool("Run", false);

                if (!(Status.MovieAct < (combatLog.Count - 1) && combatLog[Status.MovieAct + 1].subject == thisAction.subject && combatLog[Status.MovieAct + 1].action == "move"))
                    thisAction.subject.characterSound.Stop();

                numenator = 0;
                summator = Vector3.zero;
                Status.NextMovieAct();
            }

        }
        else if (thisAction.action == "attack")
        {
            if (summator == Vector3.zero)
            {
                Vector3 target;

                if (thisAction.target == null)
                {
                    target = new Vector3(CoordArray.cArray[thisAction.place[0], thisAction.place[1], 0], CoordArray.cArray[thisAction.place[0], thisAction.place[1], 1], 0);
                    summator.x = (CoordArray.cArray[thisAction.place[0], thisAction.place[1], 0] - thisAction.subject.transform.position.x) / divider;
                    summator.y = (CoordArray.cArray[thisAction.place[0], thisAction.place[1], 1] - thisAction.subject.transform.position.y) / divider;
                }
                else
                {
                    target = thisAction.target.transform.position;
                    summator.x = (thisAction.target.transform.position.x - thisAction.subject.transform.position.x) / divider;
                    summator.y = (thisAction.target.transform.position.y - thisAction.subject.transform.position.y) / divider;
                }

                //TODO - edit crutch with choosing sound
                if (thisAction.subject.ai == "")
                    if (thisAction.usedItem.rangedAttack)
                        thisAction.subject.characterSound.PlayOneShot(shootSound);
                    else
                        thisAction.subject.characterSound.PlayOneShot(hitSound);
                else
                    if (thisAction.usedItem.rangedAttack)
                    thisAction.subject.characterSound.PlayOneShot(enemyShootSound);
                else
                    thisAction.subject.characterSound.PlayOneShot(enemyHitSound);

                if (thisAction.subject.characterAnimator != null)
                {
                    TurnAnimatedObject(thisAction.subject, summator.x);
                    if (thisAction.usedItem.rangedAttack)
                    {
                        thisAction.subject.characterAnimator.SetTrigger("Shoot");
                    } else
                    {
                        thisAction.subject.characterAnimator.SetTrigger("Hit");
                    }
                }

                if (thisAction.usedItem.rangedAttack || thisAction.subject.characterAnimator == null) //TODO remove second condition
                {
                    Vector3 bulletPosition = thisAction.subject.transform.position;
                    bulletPosition.y += 0.1f;

                    bullet = Instantiate(bulletPrefab, bulletPosition, bulletPrefab.transform.rotation);
                    bullet.transform.LookAt(target);

                    int minus = 1;
                    if (bullet.transform.rotation.eulerAngles.y < 180)
                        minus = -1;
                    Quaternion correctOrientation = Quaternion.Euler(0, 0, (minus * bullet.transform.rotation.eulerAngles.x));
                    bullet.transform.rotation = correctOrientation;
                }
            }

            if (thisAction.usedItem.rangedAttack || thisAction.subject.characterAnimator == null) //TODO remove second condition
                bullet.transform.position += summator;

            if (++numenator == divider)
            {
                Destroy(bullet);
                if (thisAction.DamageDone > 0)
                {
                    thisAction.target.OverheadText.ShowRed("-" + thisAction.DamageDone);
                    thisAction.target.OverheadText.ShowHP(thisAction.targetHPAfter);
                    if (thisAction.target.characterAnimator != null && thisAction.targetHPAfter <= 0) {
                        TurnAnimatedObject(thisAction.target, -summator.x);
                        thisAction.target.characterAnimator.SetBool("Dead", true);
                    }

                }

                numenator = 0;
                summator = Vector3.zero;

                Status.NextMovieAct();
            }
        }
        else if (thisAction.action == "wait")
        {
            thisAction.subject.OverheadText.ShowGreen("+" + thisAction.apCost + " temporal AC");
            Status.NextMovieAct();
        }

        void TurnAnimatedObject(CombatCharacter animatedObject, float positiveToTheRight)
        {
            if (animatedObject.characterAnimator == null) return;
            if (positiveToTheRight > 0)
            {
                animatedObject.characterAnimator.gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
                animatedObject.characterAnimator.SetBool("ToTheLeft", false);
            }

            else
            {
                animatedObject.characterAnimator.gameObject.transform.rotation = Quaternion.Euler(0, 180, 0);
                animatedObject.characterAnimator.SetBool("ToTheLeft", true);
            }
        }

        IEnumerator PauseAndNextAct(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            Status.NextMovieAct();
        }

    }
}
