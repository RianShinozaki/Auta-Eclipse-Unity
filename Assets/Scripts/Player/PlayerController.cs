using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Rewired;
using UnityEngine.SceneManagement;
using AK.Wwise;

public class PlayerController : PhysicsEntity
{

    PlayerAnimationController animationController;

    [FoldoutGroup("Setup")] public BaseEnemy Grabbed;
    [FoldoutGroup("Setup")] public GameObject GrabTransform;
    [FoldoutGroup("Setup")] public GameObject GrabBox;
    [FoldoutGroup("Setup")] public GameObject AimArrow;

    [FoldoutGroup("Combat")] public float MaxMP;
    [FoldoutGroup("Combat")] public float MP;
    #region Movement Values

    [FoldoutGroup("Movement Values")] public float MoveSpeed;
    [FoldoutGroup("Movement Values")] public float Accel;
    [FoldoutGroup("Movement Values")] public float Stop;

    [FoldoutGroup("Movement Values")] public float JumpPower;
    [FoldoutGroup("Movement Values")] public float AirFrictionDivide;
    [FoldoutGroup("Movement Values")] public float ShortHopGravity;
    [FoldoutGroup("Movement Values")] public bool AllowShortJump;

    [FoldoutGroup("Movement Values")] public float ThrowPower;
    [FoldoutGroup("Movement Values")] public float AimDir;
    #endregion

    [FoldoutGroup("Abilities")] public bool OrbUnlocked;
    [FoldoutGroup("Abilities")] bool CanOrb;
    [FoldoutGroup("Abilities")] public bool Orb;
    [FoldoutGroup("Abilities")] public float OrbPower = 10;
    [FoldoutGroup("Abilities")] public float GhostJumpTimer = 0;
    [FoldoutGroup("Abilities")] public bool CanAttackBoost;
    [FoldoutGroup("Abilities")] public bool AttackLag;

    [FoldoutGroup("FX")] public GameObject MaterializeFX;
    [FoldoutGroup("FX")] public GameObject DisintegrateFX;
    [FoldoutGroup("FX")] public GameObject LandFX;

    public const int MOVEMENT_HORIZONTAL = 1;
    public const int MOVEMENT_VERTICAL = 2;
    public const int JUMP = 3;
    public const int ATTACK = 4;
    public const int ORB = 5;
    public const int SPECIAL = 6;
    public int PlayerID;
    [FoldoutGroup("Manual Setup")] public Player player;
    [FoldoutGroup("Manual Setup")] public StateMachine stateMachine;
    BoxCollider2D boxColl;

    float ComboNumber;
    float ComboTime;
    float AttackBuffer;
    [FoldoutGroup("Combat")] public GameObject[] HitBoxSets;
    [FoldoutGroup("Combat")] public GameObject currentHitBox = null;

    [FoldoutGroup("Sounds")] public AK.Wwise.Event Sound_OrbThrow;
    [FoldoutGroup("Sounds")] public AK.Wwise.Event Sound_Materialize;
    [FoldoutGroup("Sounds")] public AK.Wwise.Event Sound_Grab;
    [FoldoutGroup("Sounds")] public AK.Wwise.Event Sound_Throw;
    [FoldoutGroup("Sounds")] public AK.Wwise.Event Sound_SlashAttack;
    [FoldoutGroup("Sounds")] public AK.Wwise.Event Sound_Jump;
    [FoldoutGroup("Sounds")] public AK.Wwise.Event Sound_Land;

    Vector2[] CollisionSizes = new[] {
        new Vector2(0.63f,1.42f), //Normal Size
        new Vector2(0.6f,0.6f) //Orb Size
        };


    // Start is called before the first frame update
    public override void Awake()
    {
        player = ReInput.players.GetPlayer(PlayerID);

        base.Awake();

        animationController = GetComponentInChildren<PlayerAnimationController>();
        stateMachine = GetComponent<StateMachine>();
        boxColl = GetComponent<BoxCollider2D>();

        LandFX = Resources.Load<GameObject>("Player/pre_LandParticles");
    }

    private void Start()
    {
        stateMachine.SetState(State_Normal);
        stateMachine.SetRunState(true);
        ComboNumber = 1;
    }

    

    // Update is called once per frame
    public override void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        base.Update();

        stateMachine.SetRunState(HitStun == 0);

        if (HitStun > 0 && HurtState > 0)
        {
            animationController.SetSpeed(0);
            GetComponentInChildren<HurtBox>().InvincibleTime = 1;
            return;
        }

        if (HurtState > 0)
        {
            HurtState = Mathf.MoveTowards(HurtState, 0, Time.deltaTime * TimeScale * 60);
        }

        AttackBuffer = Mathf.MoveTowards(AttackBuffer, 0, Time.deltaTime);
        if(player.GetButtonDown(ATTACK))
        {
            AttackBuffer = 0.1f;
        }
        animationController.SetSpeed(TimeScale);

        if (MP < MaxMP)
            MP = Mathf.MoveTowards(MP, MaxMP, Time.deltaTime * 0.5f);
    }

    //STATES

    public void State_Normal(PhysicsEntity ent)
    {
        boxColl.size = CollisionSizes[0];

        if(Mathf.Abs(player.GetAxisRaw(MOVEMENT_HORIZONTAL)) > 0.1f) {
            Velocity.x = Mathf.MoveTowards(Velocity.x, player.GetAxisRaw(MOVEMENT_HORIZONTAL) * MoveSpeed, Accel * Time.deltaTime * TimeScale * (100/60) * (Grounded ? 1 : AirFrictionDivide) * (Underwater ? UnderwaterDragScale : 1));
        } else
        {
            if (GravityCancelTime == 0)
            {
                Velocity.x = Mathf.MoveTowards(Velocity.x, 0, Stop * Time.deltaTime * TimeScale * (100 / 60) * (Grounded ? 1 : AirFrictionDivide) * (HurtState > 0 ? 0 : 1) * (Underwater ? UnderwaterDragScale : 1));
            }
        }

        if (Grounded) {

            GhostJumpTimer = 0.11f;

            
            CanOrb = OrbUnlocked;
            CanAttackBoost = true;
        } else
        {
            GhostJumpTimer -= Time.deltaTime;
        }

        if (player.GetButtonDown(JUMP) && GhostJumpTimer > 0)
        {
            //transform.position += Vector3.up * 0.5f;
            Grounded = false;
            Velocity.y = JumpPower;
            animationController.SetJump();
            AllowShortJump = true;
            GroundPoints.Clear();

            GhostJumpTimer = 0;
            Sound_Jump.Post(gameObject);
        }

        if (player.GetButtonDown(ORB) && CanOrb)
        {
            if (Mathf.Abs(player.GetAxisRaw(MOVEMENT_HORIZONTAL)) > 0.1f || Mathf.Abs(player.GetAxisRaw(MOVEMENT_VERTICAL)) > 0.1f) {
                Velocity = new Vector2(player.GetAxisRaw(MOVEMENT_HORIZONTAL), player.GetAxisRaw(MOVEMENT_VERTICAL)).normalized * OrbPower;
                if(Grounded)
                {
                    Velocity.y = Mathf.Clamp(Velocity.y, 2f, 100f);
                }
            }
            else
            {
                Velocity = new Vector2(transform.GetChild(0).localScale.x, 0) * OrbPower;
            }
            stateMachine.SetState(State_Orb);
            

            Grounded = false;
            AllowShortJump = false;
            GroundPoints.Clear();

            CanOrb = false;
            StartCoroutine(CreateAfterImgEnum());

            GameObject img = Instantiate(DisintegrateFX, transform.position, Quaternion.identity);
            img.GetComponent<SpriteRenderer>().sprite = transform.GetChild(0).GetComponent<SpriteRenderer>().sprite;
            img.transform.localScale = transform.GetChild(0).localScale;

            Sound_OrbThrow.Post(gameObject);

        }

        if (Velocity.y > 0)
        {
            Velocity.y -= ShortHopGravity * Time.deltaTime * ((AllowShortJump && !player.GetButton(JUMP)) ? 1 : 0);
        }

        Attack();
        ComboTime = Mathf.MoveTowards(ComboTime, 0, Time.deltaTime * 10);
        if(ComboTime == 0)
        {
            ComboNumber = 1;
        }
        ComboNumber = Mathf.Clamp(ComboNumber, 1, 3);

    }

    void State_Orb(PhysicsEntity ent)
    {
        GrabBox.SetActive(true);

        boxColl.size = CollisionSizes[1];
        Orb = true;
        Bounce = true;

        if (Mathf.Abs(player.GetAxisRaw(MOVEMENT_HORIZONTAL)) > 0.1f ) {
        
            if ((Mathf.Abs(Velocity.x) < MoveSpeed) || Mathf.Sign(player.GetAxisRaw(MOVEMENT_HORIZONTAL)) != Mathf.Sign(Velocity.x))
                Velocity.x = Mathf.MoveTowards(Velocity.x, player.GetAxisRaw(MOVEMENT_HORIZONTAL) * MoveSpeed, Accel * Time.deltaTime * TimeScale * (100 / 60) * 0.15f * (Grounded ? 1 : AirFrictionDivide) * (Underwater ? UnderwaterDragScale : 1));
        }
        else
        {
            Velocity.x = Mathf.MoveTowards(Velocity.x, 0, Stop * Time.deltaTime * TimeScale * (100 / 60) * 0.15f * (Grounded ? 1 : AirFrictionDivide) * (Underwater ? UnderwaterDragScale : 1));
        }

        if(stateMachine.TimeInState > 0.4f && !player.GetButton(ORB) )
        {
            stateMachine.SetState(State_Normal);
            if( Mathf.Abs(player.GetAxisRaw(MOVEMENT_HORIZONTAL)) > 0.1f || Mathf.Abs(player.GetAxisRaw(MOVEMENT_VERTICAL)) > 0.1f)
            {
                Velocity = new Vector2(player.GetAxisRaw(MOVEMENT_HORIZONTAL), player.GetAxisRaw(MOVEMENT_VERTICAL)) * 3.5f;

            }
            Orb = false;
            Bounce = false;
            Instantiate(MaterializeFX, transform.position + Vector3.back * 0.01f, Quaternion.identity);
            Sound_Materialize.Post(gameObject);
            CreateAfterImg = false;
        }
        if(stateMachine.TimeInState > 0.08f)
        {
            Velocity.y -= Gravity * 0.5f * Time.deltaTime;
        } else
        {
            Velocity.y += Gravity * Time.deltaTime*0.8f;
        }

        if(Landed && Mathf.Abs(Velocity.y) < 3)
        {
            stateMachine.SetState(State_Normal);
            Orb = false;
            Bounce = false;
            Instantiate(MaterializeFX, transform.position + Vector3.back * 0.01f, Quaternion.identity);
            Sound_Materialize.Post(gameObject);
            CreateAfterImg = false;
        }

        Landed = false;

        if(player.GetButtonDown(ATTACK))
        {
            stateMachine.SetState(State_Normal);
            Orb = false;
            Bounce = false;
            Instantiate(MaterializeFX, transform.position + Vector3.back * 0.01f, Quaternion.identity);
            Sound_Materialize.Post(gameObject);
            CreateAfterImg = false;
            AttackBuffer = 0.1f;
        }

    }

    public void EnemyGrabbed(BaseEnemy enemy)
    {
        Sound_Grab.Post(gameObject);
        Debug.Log("test");

        Orb = false;
        Bounce = false;

        Grabbed = enemy;

        transform.position = Grabbed.transform.position;

        enemy.GrabbedBy = GetComponent<PlayerController>();
        enemy.transform.parent = GrabTransform.transform;

        stateMachine.SetState(State_GrabEnemy);

        Velocity.y = 4;
        Velocity.x = 0;

        Grounded = false;
        AllowShortJump = true;
        GroundPoints.Clear();

        CamVariables.Screenshake = 0.2f;

        GameObject hitfx = Instantiate(HitFX, transform.position, Quaternion.identity);
        hitfx.GetComponent<HitFXController>().type = EffectType.Grab;

        animationController.GrabState(true);
    }

    public void State_Attack(PhysicsEntity ent)
    {

        if (Mathf.Abs(player.GetAxisRaw(MOVEMENT_HORIZONTAL)) > 0.1f && !Grounded)
        {
            Velocity.x = Mathf.MoveTowards(Velocity.x, player.GetAxisRaw(MOVEMENT_HORIZONTAL) * MoveSpeed, Accel * Time.deltaTime * TimeScale * (60 / 60) * (Grounded ? 1 : AirFrictionDivide) * (Underwater ? UnderwaterDragScale : 1));
        }
        else
        {
            if (Mathf.Abs(Velocity.x) > 0)
            {
                Velocity.x = Mathf.MoveTowards(Velocity.x, 0, Stop * Time.deltaTime * TimeScale * (100 / 60) * (Grounded ? 1 : AirFrictionDivide) * (HurtState > 0 ? 0 : 1) * (Underwater ? UnderwaterDragScale : 1));
            }
        }

        //AnimatorClipInfo[] CurrentClipInfo;
        if (animationController.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Idle") ) {
            AttackEnd();
        }

        if (AttackLag)
            return;

        if (player.GetButtonDown(JUMP) && GhostJumpTimer > 0)
        {
            //transform.position += Vector3.up * 0.5f;
            Grounded = false;
            Velocity.y = JumpPower;
            animationController.SetJump();
            AllowShortJump = true;
            GroundPoints.Clear();
            stateMachine.SetState(State_Normal);
            GhostJumpTimer = 0;
        }

        if (player.GetButtonDown(ORB) && CanOrb)
        {
            if (Mathf.Abs(player.GetAxisRaw(MOVEMENT_HORIZONTAL)) > 0.1f || Mathf.Abs(player.GetAxisRaw(MOVEMENT_VERTICAL)) > 0.1f)
            {
                Velocity = new Vector2(player.GetAxisRaw(MOVEMENT_HORIZONTAL), player.GetAxisRaw(MOVEMENT_VERTICAL)).normalized * OrbPower;
                if (Grounded)
                {
                    Velocity.y = Mathf.Clamp(Velocity.y, 2f, 100f);
                }
            }
            else
            {
                Velocity = new Vector2(transform.GetChild(0).localScale.x, 0) * OrbPower;
            }
            stateMachine.SetState(State_Orb);


            Grounded = false;
            AllowShortJump = false;
            GroundPoints.Clear();

            CanOrb = false;
            StartCoroutine(CreateAfterImgEnum());

            GameObject img = Instantiate(DisintegrateFX, transform.position, Quaternion.identity);
            img.GetComponent<SpriteRenderer>().sprite = transform.GetChild(0).GetComponent<SpriteRenderer>().sprite;
            img.transform.localScale = transform.GetChild(0).localScale;

            Sound_OrbThrow.Post(gameObject);

        }

        if (player.GetButtonDown(ATTACK))
        {
            stateMachine.SetState(State_Normal);
            AttackBuffer = 0.1f;
        }

    }

    public void State_GrabEnemy(PhysicsEntity ent)
    {
        AimArrow.SetActive(true);

        Time.timeScale = 0.3f;

        //animationController.transform.Rotate(new Vector3(0, 0, -360 * Time.deltaTime * animationController.transform.localScale.x));

        boxColl.size = CollisionSizes[0];

        if(Mathf.Abs(player.GetAxisRaw(MOVEMENT_HORIZONTAL)) > 0.1f || Mathf.Abs(player.GetAxisRaw(MOVEMENT_VERTICAL)) > 0.1f)
        {
            AimDir = Mathf.Atan2( player.GetAxisRaw(MOVEMENT_VERTICAL), player.GetAxisRaw(MOVEMENT_HORIZONTAL) );
            AimArrow.transform.rotation = Quaternion.Euler(new Vector3(0, 0, AimDir * Mathf.Rad2Deg));
        }

        if (player.GetButtonDown(ORB))
        {
            Grabbed.stateMachine.SetState(Grabbed.State_Cannonball);

            Grabbed.Velocity.x = ThrowPower * Mathf.Cos(AimDir);
            Grabbed.Velocity.y = ThrowPower * Mathf.Sin(AimDir);

            Grabbed.transform.parent = null;
            Grabbed.GrabbedBy = null;
            Grabbed.StunTime = 0;
            Grabbed.StartCreatingAfterImgs();
            Grabbed.GravityCancelTime = 0.35f;
            GravityCancelTime = 0.35f;

            Velocity.x = -ThrowPower * Mathf.Cos(AimDir);
            Velocity.y = -ThrowPower * Mathf.Sin(AimDir);

            GrabBox.SetActive(false);

            Grabbed = null;
            animationController.GrabState(false);
            stateMachine.SetState(State_Normal);

            Time.timeScale = 1;

            AimArrow.SetActive(false);

            Sound_Throw.Post(gameObject);
        }
    }

    IEnumerator AttackPushback(float dir)
    {
        float x = 0;
        while(x < 0.05f)
        {
            x += Time.deltaTime;
            transform.position -= new Vector3(Time.deltaTime * dir * 5, 0, 0);

            yield return null;
        }
        yield break;
    }

    public void Attack()
    {
        if(AttackBuffer > 0)
        {
            Sound_SlashAttack.Post(gameObject);
            AttackLag = true;

            if (Mathf.Abs(player.GetAxisRaw(MOVEMENT_HORIZONTAL)) > 0.1f)
            {
                
                transform.GetChild(0).localScale = new Vector3(Mathf.Sign(player.GetAxisRaw(MOVEMENT_HORIZONTAL)), 1, 1);
                Velocity.x = player.GetAxisRaw(MOVEMENT_HORIZONTAL) * MoveSpeed * 1.5f;
            }
                stateMachine.SetState(State_Attack);

            if (player.GetAxisRaw(MOVEMENT_VERTICAL) > 0.1f)
            {
                animationController.SwordAttack(0, 2);
                if(CanAttackBoost && !Grounded && Velocity.y < 2.5f)
                {
                    CanAttackBoost = false;
                    Velocity.y = 2.5f;
                }
                return;
            }
            if (player.GetAxisRaw(MOVEMENT_VERTICAL) < -0.1f)
            {
                animationController.SwordAttack(0, 1);
                return;
            }

            animationController.SwordAttack(Mathf.CeilToInt(ComboNumber), 0);
            ComboNumber = Mathf.CeilToInt(ComboNumber) + 1;
            if(ComboNumber > 3)
            {
                ComboNumber = 1;
                
            }
            ComboTime = 1;

        }

        if(player.GetButtonDown(SPECIAL))
        {
            if (MP > 2)
            {
                stateMachine.SetState(State_Attack);
                animationController.GunAttack();
                MP -= 2;
                if(Grounded == false && CanAttackBoost)
                {
                    CanAttackBoost = false;
                    Velocity.y = 2;
                }
            }
        }
    }
    public override void HitResponse(GameObject attacker, GameObject Defender)
    {
        base.HitResponse(attacker, Defender);

        if (Velocity.y < 0)
            Velocity.y = 0;
        else
            Velocity.y /= 2;

        Velocity.x = 0;
        StartCoroutine(AttackPushback(Mathf.Sign(transform.GetChild(0).localScale.x)));
        if (attacker != null)
        {
            HitStun = attacker.GetComponent<HitBox>().inflictHitStun;
        }
    }
    public override void OnLand()
    {
        animationController.Land();
        Sound_Land.Post(gameObject);

        RaycastHit2D hit = Physics2D.Raycast(transform.position + Vector3.up * 0.2f, Vector3.down, 1.5f, layerMask: EnvironmentMask);
        if(hit)
        {
            Instantiate(LandFX, new Vector3(hit.point.x, hit.point.y, 0), Quaternion.identity);
        }
    }

    public void SetHitBoxes(int index)
    {
        if(HitBoxSets[index].activeSelf == true)
        {
            HitBoxSets[index].SetActive(false);
        }
        HitBoxSets[index].SetActive(true);
        currentHitBox = HitBoxSets[index];
    }

    public void AttackEnd()
    {
        stateMachine.SetState(State_Normal);
    }

    public void HideSelf()
    {
        Anim.gameObject.SetActive(false);
        stateMachine.SetRunState(false);
    }
    public void ShowSelf()
    {
        Anim.gameObject.SetActive(true);
        stateMachine.SetRunState(true);
    }
}
