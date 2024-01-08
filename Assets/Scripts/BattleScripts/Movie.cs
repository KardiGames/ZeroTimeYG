using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movie : MonoBehaviour
{
    //Settings for the movie
    private const float ACTION_TIME = 0.5f;
    float spentActionTime = 0f;

    //Loading prepetaions
    [SerializeField] private BattleManager _battleManager;
    [SerializeField] private GameObject bulletPrefab;

    [SerializeField] private AudioClip shootSound;
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private AudioClip moveSound;
    [SerializeField] private AudioClip enemyShootSound;
    [SerializeField] private AudioClip enemyHitSound;
    [SerializeField] private AudioClip enemyMoveSound;
    private GameObject bullet;
    private List<CombatAction> combatLog;
    
    Vector3 moveVector;

    private void Awake()
    {
        combatLog = _battleManager._combatLog;
    }

    // Update is called once per frame
    void Update()
    {

        if (_battleManager.Status != "movie") return;
        CombatAction thisAction = combatLog[_battleManager.MovieAct];

        if (thisAction.action == "move")
        {

            if (moveVector == Vector3.zero)
            {
                moveVector.x = CoordArray.cArray[(combatLog[_battleManager.MovieAct].place[0]), (combatLog[_battleManager.MovieAct].place[1]), 0] - combatLog[_battleManager.MovieAct].subject.transform.position.x;
                moveVector.y = CoordArray.cArray[(combatLog[_battleManager.MovieAct].place[0]), (combatLog[_battleManager.MovieAct].place[1]), 1] - combatLog[_battleManager.MovieAct].subject.transform.position.y;

                //TODO - edit crutch with choosing sound
                if (thisAction.subject._ai=="")
                    thisAction.subject.Sound.clip = moveSound;
                else
                    thisAction.subject.Sound.clip = enemyMoveSound;

                thisAction.subject.Sound.Play();

                if (thisAction.subject.CharacterAnimator != null)
                {
                    thisAction.subject.CharacterAnimator.SetBool("Run", true);
                      TurnAnimatedObject(thisAction.subject, moveVector.x);
                }

            }

            spentActionTime += Time.deltaTime;

            if (spentActionTime >= ACTION_TIME)
            {
                combatLog[_battleManager.MovieAct].subject.transform.position = new Vector3(CoordArray.cArray[(combatLog[_battleManager.MovieAct].place[0]), (combatLog[_battleManager.MovieAct].place[1]), 0], CoordArray.cArray[(combatLog[_battleManager.MovieAct].place[0]), (combatLog[_battleManager.MovieAct].place[1]), 1]); 

                if (thisAction.subject.CharacterAnimator != null && !(_battleManager.MovieAct < (combatLog.Count - 1) && combatLog[_battleManager.MovieAct + 1].subject == thisAction.subject && combatLog[_battleManager.MovieAct + 1].action == "move"))
                    thisAction.subject.CharacterAnimator.SetBool("Run", false);

                if (!(_battleManager.MovieAct < (combatLog.Count - 1) && combatLog[_battleManager.MovieAct + 1].subject == thisAction.subject && combatLog[_battleManager.MovieAct + 1].action == "move"))
                    thisAction.subject.Sound.Stop();

                spentActionTime = 0f;
                moveVector = Vector3.zero;
                _battleManager.NextMovieAct();
            } else
                combatLog[_battleManager.MovieAct].subject.transform.position += moveVector * Time.deltaTime / ACTION_TIME;

        }
        else if (thisAction.action == "attack")
        {
            if (!(thisAction.usedItem is Weapon usedWeapon))
            {
                throw new System.Exception("Item for showing Attack action is not Weapon.");
            }

            if (moveVector == Vector3.zero)
            {
                Vector3 target;

                if (thisAction.target == null)
                {
                    target = new Vector3(CoordArray.cArray[thisAction.place[0], thisAction.place[1], 0], CoordArray.cArray[thisAction.place[0], thisAction.place[1], 1], 0);
                    moveVector.x = CoordArray.cArray[thisAction.place[0], thisAction.place[1], 0] - thisAction.subject.transform.position.x;
                    moveVector.y = CoordArray.cArray[thisAction.place[0], thisAction.place[1], 1] - thisAction.subject.transform.position.y;
                }
                else
                {
                    target = thisAction.target.transform.position;
                    moveVector.x = thisAction.target.transform.position.x - thisAction.subject.transform.position.x;
                    moveVector.y = thisAction.target.transform.position.y - thisAction.subject.transform.position.y;
                }

                //TODO - edit crutch with choosing sound
                if (thisAction.subject._ai == "")
                    if (usedWeapon.RangedAttack)
                        thisAction.subject.Sound.PlayOneShot(shootSound);
                    else
                        thisAction.subject.Sound.PlayOneShot(hitSound);
                else
                    if (usedWeapon.RangedAttack)
                    thisAction.subject.Sound.PlayOneShot(enemyShootSound);
                else
                    thisAction.subject.Sound.PlayOneShot(enemyHitSound);

                if (thisAction.subject.CharacterAnimator != null)
                {
                    TurnAnimatedObject(thisAction.subject, moveVector.x);
                    if (usedWeapon.RangedAttack)
                    {
                        thisAction.subject.CharacterAnimator.SetTrigger("Shoot");
                    } else
                    {
                        thisAction.subject.CharacterAnimator.SetTrigger("Hit");
                    }
                }

                if (usedWeapon.RangedAttack || thisAction.subject.CharacterAnimator == null) //TODO remove second condition
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

            spentActionTime += Time.deltaTime;

            if (spentActionTime >= ACTION_TIME)
            {
                Destroy(bullet);
                if (thisAction.DamageDone > 0)
                {
                    thisAction.target.OverheadText.ShowRed("-" + thisAction.DamageDone);
                    thisAction.target.OverheadText.ShowHP(thisAction.TargetHPAfter);
                    if (thisAction.target.CharacterAnimator != null && thisAction.TargetHPAfter <= 0) {
                        TurnAnimatedObject(thisAction.target, -moveVector.x);
                        thisAction.target.CharacterAnimator.SetBool("Dead", true);
                    }

                }

                spentActionTime = 0f;
                moveVector = Vector3.zero;

                _battleManager.NextMovieAct();
            } else if (usedWeapon.RangedAttack || thisAction.subject.CharacterAnimator == null) //TODO remove second condition
                bullet.transform.position += moveVector * Time.deltaTime / ACTION_TIME;
        }
        else if (thisAction.action == "wait")
        {
            thisAction.subject.OverheadText.ShowGreen("+" + thisAction.apCost + " temporal AC");
            _battleManager.NextMovieAct();
        }

        void TurnAnimatedObject(CombatUnit animatedObject, float positiveToTheRight)
        {
            if (animatedObject.CharacterAnimator == null) return;
            if (positiveToTheRight > 0)
            {
                animatedObject.CharacterAnimator.gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
                animatedObject.CharacterAnimator.SetBool("ToTheLeft", false);
            }

            else
            {
                animatedObject.CharacterAnimator.gameObject.transform.rotation = Quaternion.Euler(0, 180, 0);
                animatedObject.CharacterAnimator.SetBool("ToTheLeft", true);
            }
        }

        IEnumerator PauseAndNextAct(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            _battleManager.NextMovieAct();
        }

    }
}
