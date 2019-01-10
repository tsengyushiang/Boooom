using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
    public bool networking = false;
    public bool FPSview = false;

    //component
    private Animator anim;
    private Rigidbody rig;

    //forward backward turnleft turnright
    public int latestPressbtn = -1;
    public enum inputkeys : int { UP = 0, DOWN = 1, LEFT = 2, RIGHT = 3, ATTACK = 4 };
    public KeyCode[] inputKey;

    // move speed 
    public float speed = 0.1f;
    public float turnSpeed = 400.0f;

    //game controller
    public GameObject SpawnPoint;
    public GameObject SpawnEffect;
    public GameObject DeadEffect;
    private bool isDead = false;

    public int TeamIndex;
    public GameObject bomb;
    private float MydropBombCoolDown;
   
    //被染色
    public float paintThreshold = 10f;
    public float Painted = 0;
    public int painterTeam = -1;
    public GameObject paintedeffct;

    void Start()
    {
        anim = gameObject.transform.GetChild(0).GetComponentInChildren<Animator>();
        rig = gameObject.GetComponent<Rigidbody>();

        MydropBombCoolDown = config.DROPBOMBCOOLDOWN;

        //Fetch the Renderer from the GameObject
        Renderer rend = gameObject.transform.GetChild(0).GetChild(1).GetComponent<Renderer>();
        rend.materials[1].SetColor("_Color", config.GETTEAMCOLR(TeamIndex));

        GetComponent<Painter>().team = TeamIndex;    

    }
    void Update()
    {

        if (isDead) return;

        //被其他顏色打到後被染色一陣子
        if (Painted > 0)
        {

            if (transform.position.y <= 0.6)
                Painted -= 0.2f;

            GameObject obj = Instantiate(paintedeffct);
            obj.transform.position = transform.position;
            obj.transform.parent = transform;

            ParticleSystem.MainModule newMain = obj.transform.GetChild(0).GetComponent<ParticleSystem>().main;
            newMain.startColor = config.GETTEAMCOLR(painterTeam);

            CanBePainted.PaintPoint(new Vector3(transform.position.x, 0, transform.position.z), config.GETTEAMCOLR(painterTeam), 2.5f);
        }
        else if (Painted <= 0)
        {
            //沒被染色
            GetComponent<Painter>().threshold = 0;
            painterTeam = -1;

            //案件偵測(沒被染色才能移動)
            if (networking == false)
            {
                if ((MydropBombCoolDown <= 0) && (Input.GetKey(inputKey[(int)inputkeys.ATTACK])))
                {
                    latestPressbtn = (int)inputkeys.ATTACK;
                }
                else if (Input.GetKey(inputKey[(int)inputkeys.UP]))
                {
                    latestPressbtn = (int)inputkeys.UP;
                }
                else if (Input.GetKey(inputKey[(int)inputkeys.DOWN]))
                {
                    latestPressbtn = (int)inputkeys.DOWN;
                }
                else if (Input.GetKey(inputKey[(int)inputkeys.LEFT]))
                {
                    latestPressbtn = (int)inputkeys.LEFT;
                }
                else if (Input.GetKey(inputKey[(int)inputkeys.RIGHT]))
                {
                    latestPressbtn = (int)inputkeys.RIGHT;
                }
                else
                {
                    latestPressbtn = -1;
                }
            }


            if (FPSview == false)
            {
                keydownResponse(latestPressbtn);
            }
            else {
                KeydownFPS();
            }

        }
        
        //放水球的冷卻時間
        if (MydropBombCoolDown > 0) MydropBombCoolDown -= Time.deltaTime;


    }

    public void KeydownFPS() {

        if (Input.GetKey(inputKey[(int)inputkeys.ATTACK]))
        {
            if (MydropBombCoolDown > 0) return;
            anim.Play("Jump_end");
            MydropBombCoolDown = config.DROPBOMBCOOLDOWN;
            StartCoroutine(GenerateBomb());
            latestPressbtn = -1;
        }

        if (Input.GetKey(inputKey[(int)inputkeys.UP]))
        {
            anim.SetInteger("AnimationPar", 1);            
            rig.velocity = transform.forward * speed * Time.deltaTime * 0.82f;
        }
        else
        {
            rig.velocity = Vector3.zero;
        }

        if (Input.GetKey(inputKey[(int)inputkeys.RIGHT]))
        {

            anim.SetInteger("AnimationPar", 1);

            transform.Rotate(0,1,0);

        }

        if (Input.GetKey(inputKey[(int)inputkeys.LEFT]))
        {
            anim.SetInteger("AnimationPar", 1);

            transform.Rotate(0, -1, 0);

        }
        


    }

    public  void keydownResponse(int keytype) {
        //Debug.Log(keytype);         

        if (keytype == (int)inputkeys.ATTACK)
        {
            if (MydropBombCoolDown > 0) return;
            anim.Play("Jump_end");
            MydropBombCoolDown = config.DROPBOMBCOOLDOWN;
            StartCoroutine(GenerateBomb());
            latestPressbtn = -1;
        }
        else if (keytype == (int)inputkeys.UP)
        {
            anim.SetInteger("AnimationPar", 1);
            transform.right = Vector3.forward;
            
           rig.velocity=transform.forward * speed * Time.deltaTime*0.82f;
        }
        else if (keytype == (int)inputkeys.DOWN)
        {
            anim.SetInteger("AnimationPar", 1);
            transform.right = Vector3.back;
           rig.velocity=transform.forward * speed * Time.deltaTime*0.82f;
        }
        else if (keytype == (int)inputkeys.RIGHT)
        {

            anim.SetInteger("AnimationPar", 1);
            transform.right = Vector3.right;
           rig.velocity=transform.forward * speed * Time.deltaTime*0.82f;
        }
        else if (keytype == (int)inputkeys.LEFT) {
            anim.SetInteger("AnimationPar", 1);
            transform.right = Vector3.left;
           rig.velocity=transform.forward * speed * Time.deltaTime*0.82f;
        }
        else
        {
            rig.velocity = Vector3.zero;
        }
       

    }

    IEnumerator GenerateBomb() {

        //Wait Animation
        yield return new WaitForSeconds(1.0f / 5.0f);

        GameObject newBomb = Instantiate(bomb);
        newBomb.GetComponent<Bomb>().OwnerTeam = TeamIndex;
        newBomb.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z)+transform.forward*0.3f;
        newBomb.GetComponent<Rigidbody>().AddForce(transform.forward * 200f);
    }
    public int getTeam() {
        return painterTeam == -1? TeamIndex:painterTeam;       
    }
    private void OnCollisionEnter(Collision collision)
    {
        //被水球打到
        if (collision.gameObject.GetComponent<Bomb>() != null) {
            //自己的顏色則不引響
            if (collision.gameObject.GetComponent<Painter>().team == -1) return;
            if (collision.gameObject.GetComponent<Painter>().team == getTeam()) return;
            if (Painted > 0.0f) return;
            Painted = 3.0f;
            painterTeam = collision.gameObject.GetComponent<Painter>().team;
            if(collision.gameObject.GetComponent<Rigidbody>().mass<rig.mass)
            Destroy(collision.gameObject);
        }
    }

    IEnumerator relife() {

        yield return new WaitForSeconds(2.0f);

        isDead = false;
        gameObject.SetActive(true);
        Instantiate(SpawnEffect).transform.position = SpawnPoint.transform.position;
        transform.position = SpawnPoint.transform.position;
        
    }
    public void Dead()
    {
        if (isDead == true) return; 
        Instantiate(DeadEffect).transform.position = transform.position;
        transform.position=SpawnPoint.transform.position-new Vector3(0,5,0);
        isDead = true;
        painterTeam = -1;
        Painted = 0;
        StartCoroutine(relife());
    }
}
