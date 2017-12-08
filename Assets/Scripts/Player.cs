using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Player : Character
{
    public GameObject zombie;

    public enum PlayerClass { Warrior, Mage, Rogue };

    public CharacterProperties properties;
    private float restartLevelDelay = 0.5f;
    private bool localMoving = false;
    private bool onChest = false;
    private int pressedFrames = 0;
    private int maxLevel = 1;

    public static Player Instance = null;
    [HideInInspector]
    public List<Vector2> destination = new List<Vector2>();
    [HideInInspector]
    public float currentExp = 0;
    [HideInInspector]
    public Armor set;
    [HideInInspector]
    public Weapon weapon;
    [HideInInspector]
    public int currentMoves;
    [HideInInspector]
    public Text mainText;
    [HideInInspector]
    public bool ChestMenuOn = false;
    [HideInInspector]
    public bool IsUpdateBlocked = false;
    [HideInInspector]
    public PlayerClass Class;
    [HideInInspector]
    public int Level;
    [HideInInspector]
    public int gold;
    [HideInInspector]
    public int skillPoints;
    [HideInInspector]
    public int damage;
    [HideInInspector]
    public int armor;
    [HideInInspector]
    public int movesPerRound;
    [HideInInspector]
    public float damageAmplifier = 1;
    [HideInInspector]
    public float defenceAmplifier = 1;
    public Equipment equipment;
    public EndGameInfo EndGameInfo;

    public ShieldBash ShieldBash;
    public BrutalAttack BrutalAttack;
    public WideAttack WideAttack;
    public PushingAttack PushingAttack;
    public BerserkMode BerserkMode;

    public PoisonTrap PoisonTrap;
    public Invisibility Invisibility;
    public Snare Snare;
    public Multishot Multishot;
    public Decoy Decoy;

    public PoisonMissile PoisonMissile;
    public Thunderbolt Thunderbolt;
    public Glyph Glyph;
    public LifeDrain LifeDrain;
    public Sacrifice Sacrifice;

    public RuntimeAnimatorController warrior1Controller;
    public RuntimeAnimatorController warrior2Controller;
    public RuntimeAnimatorController warrior1BerserkerController;
    public RuntimeAnimatorController warrior2BerserkerController;
    public RuntimeAnimatorController rogue1Controller;
    public RuntimeAnimatorController rogue2Controller;
    public RuntimeAnimatorController mage1Controller;
    public RuntimeAnimatorController mage2Controller;

    public Camera mainCamera;
    public AudioClip[] footsteps;
    public AudioClip sword;

    public Image HealthBar;
    public Image ManaBar;
    public Rigidbody2D arrow;
    public Rigidbody2D magicMissile;
    public Text floatingText;

    /**************************/

    protected override void Awake()
    {
        DontDestroyOnLoad(gameObject);

        Instance = this;
        base.Awake();
        name = "Player";
    }

    private void Attack(GenericEnemy enemy)
    {
        bool attacked = false;

        if (GameManager.Instance.NormalAttack)
        {
            NormalAttack(enemy);
            attacked = true;
        }
        else if (GameManager.Instance.ShieldBash)
        {
            ShieldBash.Use(enemy);
            attacked = true;
        }
        else if (GameManager.Instance.BrutalAttack)
        {
            BrutalAttack.Use(enemy);
            attacked = true;
        }
        else if (GameManager.Instance.WideAttack)
        {
            WideAttack.Use(enemy);
            attacked = true;
        }
        else if (GameManager.Instance.PushingAttack)
        {
            PushingAttack.Use(enemy);
            attacked = true;
        }
        else if (GameManager.Instance.Multishot)
        {
            Multishot.Use(enemy);
            attacked = true;
        }
        else if(GameManager.Instance.Thunderbolt)
        {
            Thunderbolt.Use(enemy);
            attacked = true;
        }
        else if (GameManager.Instance.PoisonMissile)
        {
            PoisonMissile.Use(enemy);
            attacked = true;
        }
        else if (GameManager.Instance.LifeDrain)
        {
            LifeDrain.Use(enemy);
            attacked = true;
        }

        if (attacked)
        {
            if(Invisible)
            {
                for (int i = 0; i < Invisibility.cooldown; i++)
                    Invisibility.DecreaseTurns();
            }
        }
    }


    public void BlockUpdate()
    {
        IsUpdateBlocked = true;
    }


    public bool CanShoot(Vector2 targetPosition)
    {
        RaycastHit2D hit;

        boxCollider.enabled = false;
        hit = Physics2D.Linecast(transform.position, targetPosition, blockingLayer);
        boxCollider.enabled = true;

        if (hit.collider.tag == "Enemy")
            return true;

        return false;
    }

    public void CameraSize(float size)
    {
        mainCamera.orthographicSize = size;
    }

    public void ChangeClass(string playerClass)
    {
        if (GameManager.Instance.ActiveScene.name.Equals("Camp"))
        {
            if (playerClass.Equals("Warrior") && Class != PlayerClass.Warrior)
                ChangeClassToWarrior();
            else if (playerClass.Equals("Rogue") && Class != PlayerClass.Rogue)
                ChangeClassToRogue();
            else if (playerClass.Equals("Mage") && Class != PlayerClass.Mage)
                ChangeClassToMage();
        }
    }

    private void ChangeClassToWarrior()
    {
        Class = PlayerClass.Warrior;

        armor = 0;
        damage = 0;
        SetStats();

        set = GameManager.Instance.warriorSet;
        weapon.equiped = false;

        if (GameManager.Instance.warriorItems.Count > 0)
            weapon = GameManager.Instance.warriorItems[0];
        else
            weapon = GameManager.Instance.hand;

        weapon.equiped = true;
        armor += equipment.GetBonusArmor();
        damage += equipment.GetBonusDamage();

        if (weapon.WeponName.Equals("One handed sword"))
            ChangeWeaponToOneHandedSword();
        else if (weapon.WeponName.Equals("Two handed sword"))
            ChangeWeaponToTwoHandedSword();

        GameManager.Instance.Skill3Image.sprite = BerserkMode.Image;

        if (BerserkMode.cooldown != 0)
            GameManager.Instance.Skill3Text.text = BerserkMode.cooldown.ToString();
        else
            GameManager.Instance.Skill3Text.text = "";

        GameManager.Instance.ShowPlayer();
    }

    private void ChangeClassToRogue()
    {
        Class = PlayerClass.Rogue;

        armor = 0;
        damage = 0;
        SetStats();

        set = GameManager.Instance.rogueSet;
        weapon.equiped = false;

        if (GameManager.Instance.rogueItems.Count > 0)
            weapon = GameManager.Instance.rogueItems[0];
        else
            weapon = GameManager.Instance.hand;

        weapon.equiped = true;
        armor += equipment.GetBonusArmor();
        damage += equipment.GetBonusDamage();

        if (weapon.WeponName.Equals("Daggers"))
            ChangeWeaponToDagger();     
        else if (weapon.WeponName.Equals("Bow"))
            ChangeWeaponToBow();

        GameManager.Instance.Skill3Image.sprite = Decoy.Image;

        if (Decoy.cooldown != 0)
            GameManager.Instance.Skill3Text.text = Decoy.cooldown.ToString();
        else
            GameManager.Instance.Skill3Text.text = "";

        GameManager.Instance.ShowPlayer();
    }

    private void ChangeClassToMage()
    {
        Class = PlayerClass.Mage;

        armor = 0;
        damage = 0;
        SetStats();

        set = GameManager.Instance.mageSet;
        weapon.equiped = false;

        if (GameManager.Instance.mageItems.Count > 0)
            weapon = GameManager.Instance.mageItems[0];
        else
            weapon = GameManager.Instance.hand;

        weapon.equiped = true;
        armor += equipment.GetBonusArmor();
        damage += equipment.GetBonusDamage();

        if (weapon.WeponName.Equals("Staff"))
            ChangeWeaponToStaff();
        else if (weapon.WeponName.Equals("Rod"))
            ChangeWeaponToRod();

        GameManager.Instance.Skill3Image.sprite = Sacrifice.Image;

        if (Sacrifice.cooldown != 0)
            GameManager.Instance.Skill3Text.text = Sacrifice.cooldown.ToString();
        else
            GameManager.Instance.Skill3Text.text = "";

        GameManager.Instance.ShowPlayer();
    }

    public void ChangeWeaponToOneHandedSword()
    {
        animator.runtimeAnimatorController = warrior1Controller;
        set.firStyleActive = true;

        GameManager.Instance.Skill1Image.sprite = ShieldBash.Image;
        GameManager.Instance.Skill2Image.sprite = BrutalAttack.Image;

        if (ShieldBash.cooldown != 0)
            GameManager.Instance.Skill1Text.text = ShieldBash.cooldown.ToString();
        else
            GameManager.Instance.Skill1Text.text = "";

        if (BrutalAttack.cooldown != 0)
            GameManager.Instance.Skill2Text.text = BrutalAttack.cooldown.ToString();
        else
            GameManager.Instance.Skill2Text.text = "";

        GameManager.Instance.AttackImage.sprite = weapon.image;
    }

    public void ChangeWeaponToTwoHandedSword()
    {
        animator.runtimeAnimatorController = warrior2Controller;
        set.firStyleActive = false;

        GameManager.Instance.Skill1Image.sprite = WideAttack.Image;
        GameManager.Instance.Skill2Image.sprite = PushingAttack.Image;

        if (WideAttack.cooldown != 0)
            GameManager.Instance.Skill1Text.text = WideAttack.cooldown.ToString();
        else
            GameManager.Instance.Skill1Text.text = "";

        if (PushingAttack.cooldown != 0)
            GameManager.Instance.Skill2Text.text = PushingAttack.cooldown.ToString();
        else
            GameManager.Instance.Skill2Text.text = "";

        GameManager.Instance.AttackImage.sprite = weapon.image;
    }

    public void ChangeWeaponToDagger()
    {
        animator.runtimeAnimatorController = rogue1Controller;
        set.firStyleActive = true;

        GameManager.Instance.Skill1Image.sprite = PoisonTrap.Image;
        GameManager.Instance.Skill2Image.sprite = Invisibility.Image;

        if (PoisonTrap.cooldown != 0)
            GameManager.Instance.Skill1Text.text = PoisonTrap.cooldown.ToString();
        else
            GameManager.Instance.Skill1Text.text = "";

        if (Invisibility.cooldown != 0)
            GameManager.Instance.Skill2Text.text = Invisibility.cooldown.ToString();
        else
            GameManager.Instance.Skill2Text.text = "";

        GameManager.Instance.AttackImage.sprite = weapon.image;
    }

    public void ChangeWeaponToBow()
    {
        animator.runtimeAnimatorController = rogue2Controller;
        set.firStyleActive = false;

        GameManager.Instance.Skill1Image.sprite = Snare.Image;
        GameManager.Instance.Skill2Image.sprite = Multishot.Image;

        if (Snare.cooldown != 0)
            GameManager.Instance.Skill1Text.text = Snare.cooldown.ToString();
        else
            GameManager.Instance.Skill1Text.text = "";

        if (Multishot.cooldown != 0)
            GameManager.Instance.Skill2Text.text = Multishot.cooldown.ToString();
        else
            GameManager.Instance.Skill2Text.text = "";

        GameManager.Instance.AttackImage.sprite = weapon.image;
    }

    public void ChangeWeaponToStaff()
    {
        animator.runtimeAnimatorController = mage1Controller;
        set.firStyleActive = true;

        GameManager.Instance.Skill1Image.sprite = Glyph.Image;
        GameManager.Instance.Skill2Image.sprite = LifeDrain.Image;

        if (Glyph.cooldown != 0)
            GameManager.Instance.Skill1Text.text = Glyph.cooldown.ToString();
        else
            GameManager.Instance.Skill1Text.text = "";

        if (LifeDrain.cooldown != 0)
            GameManager.Instance.Skill2Text.text = LifeDrain.cooldown.ToString();
        else
            GameManager.Instance.Skill2Text.text = "";

        GameManager.Instance.AttackImage.sprite = weapon.image;
    }

    public void ChangeWeaponToRod()
    {
        animator.runtimeAnimatorController = mage2Controller;
        set.firStyleActive = false;

        GameManager.Instance.Skill1Image.sprite = Thunderbolt.Image;
        GameManager.Instance.Skill2Image.sprite = PoisonMissile.Image;

        if (Thunderbolt.cooldown != 0)
            GameManager.Instance.Skill1Text.text = Thunderbolt.cooldown.ToString();
        else
            GameManager.Instance.Skill1Text.text = "";

        if (PoisonMissile.cooldown != 0)
            GameManager.Instance.Skill2Text.text = PoisonMissile.cooldown.ToString();
        else
            GameManager.Instance.Skill2Text.text = "";

        GameManager.Instance.AttackImage.sprite = weapon.image;
    }


    private void DoMoveTurn()
    {
        if (!moving || destination.Count > 0)
        {
            if (!moving && !localMoving)
                GetMoveInput();

            if (destination.Count > 0 && !localMoving)
            {
                localMoving = true;
                StartCoroutine(MoveOneUnit(destination[0]));
                destination.RemoveAt(0);

                if (GameManager.Instance.InFight)
                {
                    currentMoves--;
                    GameManager.Instance.MovesText.text = "Moves: " + currentMoves.ToString();

                    if (currentMoves == 0) GameManager.Instance.CurrentTurn = GameManager.Turns.Actions;
                }
            }
            else
            {
                if (Input.GetMouseButtonDown(0) || Input.touchCount == 1)
                    StopMoving();
            }

            if (destination.Count == 0)
                moving = false;
        }
    }

    private void DoAttackTurn()
    {
        GetAttackMouseInput();
        GetAttackTouchInput();
    }


    private void EnemiesInRange()
    {
        List<Vector2> path = new List<Vector2>();

        foreach (var enemy in GameManager.Instance.enemies)
        {
            if (enemy.enabled == true) continue;

            var enemyTransform = enemy.GetComponent<Transform>();

            float distanceX = Math.Abs(enemyTransform.position.x - transform.position.x);
            float distanceY = Math.Abs(enemyTransform.position.y - transform.position.y);

            if (distanceX < 7 && distanceY < 5)
            {
                BoardManager.obstacles[(int)enemyTransform.position.x][(int)enemyTransform.position.y] = '-';
                path = shortestPath(BoardManager.obstacles, (int)transform.position.x, (int)transform.position.y, (int)enemyTransform.position.x, (int)enemyTransform.position.y);
                BoardManager.obstacles[(int)enemyTransform.position.x][(int)enemyTransform.position.y] = 'X';

                if (path.Count > 0 && path.Count < 10)
                {
                    enemy.enabled = true;
                    GameManager.Instance.InFight = true;
                    GameManager.Instance.TurnText.text = "Player Turn";
                    GameManager.Instance.MovesText.text = "Moves: " + currentMoves.ToString();
                    GameManager.Instance.EndTurnButton.interactable = true;
                }
            }
        }
    }

    public void EquipItem()
    {
        if (!GameManager.Instance.InFight)
        {
            if (Class == PlayerClass.Warrior && GameManager.Instance.currentTab == GameManager.Tabs.WarriorItems)
            {
                weapon.equiped = false;
                damage -= equipment.GetBonusDamage();

                weapon = GameManager.Instance.warriorItems[GameManager.Instance.selectedItemSlot];
                equipment.weapon = GameManager.Instance.warriorItems[GameManager.Instance.selectedItemSlot];

                damage += equipment.GetBonusDamage();
                weapon.equiped = true;

                if (weapon.WeponName.Equals("One handed sword"))
                    ChangeWeaponToOneHandedSword();
                else if (weapon.WeponName.Equals("Two handed sword"))
                    ChangeWeaponToTwoHandedSword();
            }
            else if (Class == PlayerClass.Rogue && GameManager.Instance.currentTab == GameManager.Tabs.RogueItems)
            {
                weapon.equiped = false;
                damage -= equipment.GetBonusDamage();

                weapon = GameManager.Instance.rogueItems[GameManager.Instance.selectedItemSlot];
                equipment.weapon = GameManager.Instance.rogueItems[GameManager.Instance.selectedItemSlot];

                damage += equipment.GetBonusDamage();
                weapon.equiped = true;

                if (weapon.WeponName.Equals("Daggers"))
                    ChangeWeaponToDagger();
                else if (weapon.WeponName.Equals("Bow"))
                    ChangeWeaponToBow();
            }
            else if (Class == PlayerClass.Mage && GameManager.Instance.currentTab == GameManager.Tabs.MageItems)
            {
                weapon.equiped = false;
                damage -= equipment.GetBonusDamage();

                weapon = GameManager.Instance.mageItems[GameManager.Instance.selectedItemSlot];
                equipment.weapon = GameManager.Instance.mageItems[GameManager.Instance.selectedItemSlot];

                damage += equipment.GetBonusDamage();
                weapon.equiped = true;
                if (weapon.WeponName.Equals("Staff"))
                    ChangeWeaponToStaff();
                else if (weapon.WeponName.Equals("Rod"))
                    ChangeWeaponToRod();
            }

            GameManager.Instance.ItemDetails(GameManager.Instance.selectedItemSlot);
        }
    }

    private GenericEnemy EnemyClicked(Vector2 clicked)
    {
        var enabledEnemies = GameManager.Instance.enemies.Where(enemy => enemy.enabled);
        return enabledEnemies.FirstOrDefault(enemy =>
                                            (enemy.transform.position.x == (int)clicked.x + 0.5f &&
                                             enemy.transform.position.y == (int)clicked.y + 0.5f));
    }

    public void Equip()
    {
        equipment = new Equipment(weapon, set);

        armor += equipment.GetBonusArmor();
        damage += equipment.GetBonusDamage();
    }


    public void GainExp(int exp)
    {
        currentExp += exp;
        float expToNextLevel = Level * 100;
      
        if (currentExp >= expToNextLevel)
        {
            currentExp -= expToNextLevel;
            LevelUp();
            expToNextLevel = Level * 100;
        }
           
        float fill = currentExp / expToNextLevel;

        GameManager.Instance.CurrentExperience.text = "Current experience points: " + currentExp;
        GameManager.Instance.ExperienceToNextLevel.text = "Experience to next level: " + (expToNextLevel - currentExp);
        GameManager.Instance.ExperienceBar.fillAmount = fill;
    }

    public void GainStrenght(int strength)
    {
        properties.StrengthPoints += strength;
        properties.HealthPoints += strength * 3;
        currentHealthPoints += strength * 3;
        if (Class == PlayerClass.Warrior) damage += strength;
    }

    public void GainAgility(int agility)
    {
        properties.AgilityPoints += agility;
        armor = (properties.AgilityPoints / 5) + weapon.armor + set.armor;
        if (Class == PlayerClass.Rogue) damage += agility;
    }

    public void GainIntelligence(int intelligence)
    {
        properties.IntelligencePoints += intelligence;
        properties.ManaPoints += intelligence * 2;
        currentManaPoints += intelligence * 2;
        if (Class == PlayerClass.Mage) damage += intelligence;
    }

    private void GetMoveInput()
    {
        GetMoveKeyboardInput(destination);
        GetMoveMouseInput(destination);
        GetMoveTouchInput(destination);

        if (destination.Count > currentMoves && GameManager.Instance.InFight)
            destination.RemoveRange(currentMoves, destination.Count - currentMoves);
        if (destination.Count > 0)
            moving = true;
    }

    private void GetMoveKeyboardInput(List<Vector2> destination)
    {
        if (Input.GetAxisRaw("Vertical") == 1 && CanMove(0, 1)) destination.Add(new Vector2(0, 1));
        if (Input.GetAxisRaw("Vertical") == -1 && CanMove(0, -1)) destination.Add(new Vector2(0, -1));
        if (Input.GetAxisRaw("Horizontal") == 1 && CanMove(1, 0)) destination.Add(new Vector2(1, 0));
        if (Input.GetAxisRaw("Horizontal") == -1 && CanMove(-1, 0)) destination.Add(new Vector2(-1, 0));
    }

    private void GetMoveMouseInput(List<Vector2> destination)
    {
        if (Input.touchCount == 0)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (!EventSystem.current.IsPointerOverGameObject())
                {
                    Vector2 clicked = mainCamera.ScreenToWorldPoint(Input.mousePosition);

                    GenericEnemy enemy = EnemyClicked(clicked);

                    if (enemy != null)
                        Attack(enemy);
                    else
                    {
                        if (GameManager.Instance.ActiveScene.name.Equals("Camp"))
                            destination = shortestPath(GameManager.Instance.Obsacles,
                                                      (int)transform.position.x,
                                                      (int)transform.position.y,
                                                      (int)clicked.x,
                                                      (int)clicked.y);
                        else
                            destination = shortestPath(BoardManager.obstacles,
                                                      (int)transform.position.x,
                                                      (int)transform.position.y,
                                                      (int)clicked.x,
                                                      (int)clicked.y);

                        this.destination.AddRange(destination);
                    }
                }
            }
        }
    }

    private void GetMoveTouchInput(List<Vector2> destination)
    {
        if (Input.touchCount == 1)
        {
            Touch myTouch = Input.touches[0];

            pressedFrames++;

            if (!IsPointerOverUIObject(new Vector2(myTouch.position.x, myTouch.position.y)))
            {
                if (myTouch.phase == TouchPhase.Ended)
                {
                    Vector2 clicked = mainCamera.ScreenToWorldPoint(new Vector3(myTouch.position.x,
                                                                                myTouch.position.y,
                                                                                mainCamera.nearClipPlane));
                    GenericEnemy enemy = EnemyClicked(clicked);

                    if (enemy != null)
                        Attack(enemy);
                    else
                    {
                        if (GameManager.Instance.ActiveScene.name.Equals("Camp"))
                            destination = shortestPath(GameManager.Instance.Obsacles,
                                                      (int)transform.position.x,
                                                      (int)transform.position.y,
                                                      (int)clicked.x,
                                                      (int)clicked.y);
                        else
                            destination = shortestPath(BoardManager.obstacles,
                                                      (int)transform.position.x,
                                                      (int)transform.position.y,
                                                      (int)clicked.x,
                                                      (int)clicked.y);

                        this.destination.AddRange(destination);
                    }

                    pressedFrames = 0;
                }
            }
        }
    }

    private void GetAttackMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 clicked = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            GenericEnemy enemy = EnemyClicked(clicked);

            if (enemy != null)
                Attack(enemy);
        }
    }

    private void GetAttackTouchInput()
    {
        if (Input.touchCount == 1)
        {
            Touch myTouch = Input.touches[0];

            if (myTouch.phase == TouchPhase.Ended)
            {
                Vector2 clicked = mainCamera.ScreenToWorldPoint(new Vector3(myTouch.position.x,
                                                                            myTouch.position.y,
                                                                            mainCamera.nearClipPlane));
                GenericEnemy enemy = EnemyClicked(clicked);

                if (enemy != null)
                    Attack(enemy);
            }
        }
    }

    private void GameOver()
    {
        StopAllCoroutines();
        GameManager.Instance.StopAllCoroutines();

        EndGameInfo.LevelGained = Level;
        EndGameInfo.DungeonLevel = GameManager.Instance.level;
        
        currentMoves = movesPerRound;
        currentHealthPoints = properties.HealthPoints;
        currentManaPoints = properties.ManaPoints;
        currentExp = 0;

        Level = 1;
        damage = weapon.damage;

        SetStats();
        GainExp(0);

        GameManager.Instance.ShowEndingScreen();
        GameManager.Instance.level = 0;
        GameManager.Instance.enemies.Clear();
        GameManager.Instance.SaveGame(true);
        GameManager.Instance.EndFight();
        GameManager.Instance.ChangeAlphaOfImage(GameManager.Instance.Skill1Image, 1);
        GameManager.Instance.ChangeAlphaOfImage(GameManager.Instance.Skill2Image, 1);
        GameManager.Instance.ChangeAlphaOfImage(GameManager.Instance.Skill3Image, 1);

        RestartSkills();
        skillPoints = maxLevel;
    }


    private bool IsPointerOverUIObject(Vector2 position)
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = position;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

        return results.Count > 0;
    }

    private bool IsUpdateValid()
    {
        if (!GameManager.Instance.PlayerTurn ||
            GameManager.Instance.MenuOn ||
            ChestMenuOn ||
            IsUpdateBlocked) return false;

        return true;
    }


    private void LevelUp()
    {
        if (Class == PlayerClass.Warrior)
        {
            GainStrenght(3);
            GainAgility(2);
            GainIntelligence(1);
        }
        else if (Class == PlayerClass.Rogue)
        {
            GainStrenght(2);
            GainAgility(3);
            GainIntelligence(1);
        }
        else if (Class == PlayerClass.Mage)
        {
            GainStrenght(2);
            GainAgility(1);
            GainIntelligence(3);
        }

        Level++;

        if (Level > maxLevel)
        {
            maxLevel = Level;
            skillPoints++;
        }

        UpdatePlayerHealth();
        UpdatePlayerMana();
    }

    public void LoadStats()
    {
        Level = PlayerPrefs.GetInt("PlayerLeve");
        gold = PlayerPrefs.GetInt("Gold");
        currentExp = PlayerPrefs.GetInt("Exp");
        maxLevel = PlayerPrefs.GetInt("MaxLevel");
        skillPoints = maxLevel;

        SetStats();
        for (int i = 1; i < Level; i++) LevelUp();

        currentMoves = movesPerRound;
        currentHealthPoints = properties.HealthPoints;
        currentManaPoints = properties.ManaPoints;

        float expToNextLevel = Level * 200 + (Level - 1) * 50;
        float fill = currentExp / expToNextLevel;

        GameManager.Instance.CurrentExperience.text = "Current experience points: " + currentExp;
        GameManager.Instance.ExperienceToNextLevel.text = "Experience to next level: " + (expToNextLevel - currentExp);
        GameManager.Instance.ExperienceBar.fillAmount = fill;

        if (Class == PlayerClass.Warrior)
            GameManager.Instance.Skill3Image.sprite = BerserkMode.Image;
        else if (Class == PlayerClass.Rogue)
            GameManager.Instance.Skill3Image.sprite = Decoy.Image;
        else if (Class == PlayerClass.Mage)
            GameManager.Instance.Skill3Image.sprite = Sacrifice.Image;
    }

    public void LoadSkills()
    {
        ShieldBash.Load();
        BrutalAttack.Load();
        WideAttack.Load();
        PushingAttack.Load();
        BerserkMode.Load();

        PoisonTrap.Load();
        Invisibility.Load();
        Snare.Load();
        Multishot.Load();
        Decoy.Load();

        Thunderbolt.Load();
        PoisonMissile.Load();
        Glyph.Load();
        LifeDrain.Load();
        Sacrifice.Load();
    }

    public void LoadUiState()
    {
        if (GameManager.Instance.level > 1)
        {
            if (Class == PlayerClass.Warrior)
            {
                if (ShieldBash.Lvl == 5)
                    GameManager.Instance.UpgradeSkill1Button.interactable = false;
                else if (BrutalAttack.Lvl == 5)
                    GameManager.Instance.UpgradeSkill2Button.interactable = false;
                else if (WideAttack.Lvl == 5)
                    GameManager.Instance.UpgradeSkill3Button.interactable = false;
                else if (PushingAttack.Lvl == 5)
                    GameManager.Instance.UpgradeSkill4Button.interactable = false;
                else if (BerserkMode.Lvl == 5)
                    GameManager.Instance.UpgradeSkill5Button.interactable = false;
            }
            else if (Class == PlayerClass.Rogue)
            {
                if (PoisonTrap.Lvl == 5)
                    GameManager.Instance.UpgradeSkill1Button.interactable = false;
                else if (Invisibility.Lvl == 5)
                    GameManager.Instance.UpgradeSkill2Button.interactable = false;
                else if (Snare.Lvl == 5)
                    GameManager.Instance.UpgradeSkill3Button.interactable = false;
                else if (Multishot.Lvl == 5)
                    GameManager.Instance.UpgradeSkill4Button.interactable = false;
                else if (Decoy.Lvl == 5)
                    GameManager.Instance.UpgradeSkill5Button.interactable = false;
            }
            else if (Class == PlayerClass.Mage)
            {
                if (Thunderbolt.Lvl == 5)
                    GameManager.Instance.UpgradeSkill1Button.interactable = false;
                else if (PoisonMissile.Lvl == 5)
                    GameManager.Instance.UpgradeSkill2Button.interactable = false;
                else if (Glyph.Lvl == 5)
                    GameManager.Instance.UpgradeSkill3Button.interactable = false;
                else if (LifeDrain.Lvl == 5)
                    GameManager.Instance.UpgradeSkill4Button.interactable = false;
                else if (Sacrifice.Lvl == 5)
                    GameManager.Instance.UpgradeSkill5Button.interactable = false;
            }
        }
    }


    public IEnumerator MoveOneUnit(Vector2 destination)
    {
        Vector2 end;

        if (CanMove((int)destination.x, (int)destination.y, out end))
        {
            float sqrRemainingDistance = (end - rb2D.position).sqrMagnitude;
            SoundManager.Instance.RandomizeSfx(footsteps);

            while (sqrRemainingDistance > float.Epsilon)
            {
                Vector2 newPostion = 
                    Vector2.MoveTowards(rb2D.position, end, 4 * Time.deltaTime);

                rb2D.MovePosition(newPostion);
                sqrRemainingDistance = (end - rb2D.position).sqrMagnitude;
                SetCameraAndFloatingText();

                yield return null;
            }
        }

        if (GameManager.Instance.ActiveScene.name.Equals("Anomaly"))
            EnemiesInRange();

        localMoving = false;
    }


    private void NormalAttack(GenericEnemy enemy)
    {
        Vector2 targetPosition = enemy.transform.position;
        Vector2 distance = targetPosition - (Vector2)transform.position;
        float sqrDistance = distance.sqrMagnitude;

        if (weapon.type == Weapon.Types.Melee && sqrDistance <= 2)
        {
            animator.SetTrigger("attack");
            enemy.TakeDamage(damage);
            SoundManager.Instance.PlaySingle(sword);
            GameManager.Instance.ChangeTurnDelayed();
        }
        else if (weapon.type == Weapon.Types.Ranged && CanShoot(targetPosition))
        {
            animator.SetTrigger("attack");

            if (Class == PlayerClass.Rogue)
                Shoot(targetPosition, arrow, damage);
            else if (Class == PlayerClass.Mage)
                Shoot(targetPosition, magicMissile, damage);

            SoundManager.Instance.RandomizeSfx(shoot);
            BlockUpdate();
        } 
    }


    private void OnLevelWasLoaded(int index)
    {
        if (index == 1)
        {
            transform.position = new Vector2(9.5f, 20.5f);
            SetCameraAndFloatingText();
            moving = false;
            localMoving = false;
            GameManager.Instance.InFight = false;
            RestartCooldowns();
        }
        else if (index == 2)
            enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Exit")
        {
            GameManager.Instance.InGameUI.SetActive(false);
            GameManager.Instance.LoadingScreen.SetActive(true);

            GameManager.Instance.DungeonLevel.text = "Level: " + (GameManager.Instance.level + 1).ToString();
            Invoke("Restart", restartLevelDelay);

            enabled = false;
        }
        else if (other.tag == "Chest")
        {
            onChest = !onChest;

            if (onChest)
                OpenChest();
            else
                other.enabled = false;
        }
    }

    private void OpenChest()
    {
        ChestMenuOn = true;

        if (GameManager.Instance.ShowUI.activeSelf)
            GameManager.Instance.ShowUI.SetActive(false);
        else
            GameManager.Instance.InGameUI.SetActive(false);

        GameManager.Instance.Chest.SetActive(true);
        GameManager.Instance.RandomItems();
    }


    private void Restart()
    {
        StopAllCoroutines();
        GameManager.Instance.StopAllCoroutines();

        StopMoving();
        GameManager.Instance.enemies.Clear();
        SceneManager.LoadScene("Anomaly");
    }

    public void RestartCooldowns()
    {
        ShieldBash.cooldown = 0;
        BrutalAttack.cooldown = 0;
        WideAttack.cooldown = 0;
        PushingAttack.cooldown = 0;
        BerserkMode.cooldown = 0;

        PoisonTrap.cooldown = 0;
        Invisibility.cooldown = 0;
        Snare.cooldown = 0;
        Multishot.cooldown = 0;
        Decoy.cooldown = 0;

        Thunderbolt.cooldown = 0;
        PoisonMissile.cooldown = 0;
        Glyph.cooldown = 0;
        LifeDrain.cooldown = 0;
        Sacrifice.cooldown = 0;

        GameManager.Instance.Skill1Text.text = "";
        GameManager.Instance.Skill2Text.text = "";
        GameManager.Instance.Skill3Text.text = "";

        GameManager.Instance.ChangeAlphaOfImage(GameManager.Instance.Skill1Image, 1);
        GameManager.Instance.ChangeAlphaOfImage(GameManager.Instance.Skill2Image, 1);
        GameManager.Instance.ChangeAlphaOfImage(GameManager.Instance.Skill3Image, 1);
    }

    public void RestartSkills()
    {
        ShieldBash.Restart();
        BrutalAttack.Restart();
        WideAttack.Restart();
        PushingAttack.Restart();
        BerserkMode.Restart();

        PoisonTrap.Restart();
        Invisibility.Restart();
        Snare.Restart();
        Multishot.Restart();
        Decoy.Restart();

        Thunderbolt.Restart();
        PoisonMissile.Restart();
        Glyph.Restart();
        LifeDrain.Restart();
        Sacrifice.Restart();
    }


    private void Start()
    {
        //Transform objectsHolder = GameManager.Instance.InGameUI.transform;
        //mainText = Instantiate(floatingText,new Vector3(0,0,0), Quaternion.identity) as Text;
        //mainText.transform.SetParent(objectsHolder, false);
    }

    public void StopMoving()
    {
        destination.Clear();
        moving = false;
    }

    public void SetCameraAndFloatingText()
    {
        mainCamera.transform.position = new Vector3(transform.position.x, transform.position.y, -1);
        //mainText.transform.position = transform.position + new Vector3(0, 0.6f, 0);
    }

    public void SetStats()
    {
        if (Class == PlayerClass.Warrior)
        {
            properties.Restart();
            properties.HealthPoints = 700;
            properties.ManaPoints = 150;
            GainStrenght(20);
            GainAgility(13);
            GainIntelligence(7);
            movesPerRound = 2;
        }
        else if (Class == PlayerClass.Rogue)
        {
            properties.Restart();
            properties.HealthPoints = 550;
            properties.ManaPoints = 150;
            GainStrenght(13);
            GainAgility(20);
            GainIntelligence(7);
            movesPerRound = 3;
        }
        else if (Class == PlayerClass.Mage)
        {
            properties.Restart();
            properties.HealthPoints = 450;
            properties.ManaPoints = 250;
            GainStrenght(10);
            GainAgility(10);
            GainIntelligence(20);
            movesPerRound = 2;
        }

        currentMoves = movesPerRound;
        currentHealthPoints = properties.HealthPoints;
        currentManaPoints = properties.ManaPoints;

        UpdatePlayerHealth();
        UpdatePlayerMana();
    }

    public void SaveStats()
    {
        PlayerPrefs.SetString("Class", Class.ToString());
        PlayerPrefs.SetInt("PlayerLeve", Level);
        PlayerPrefs.SetInt("Gold", gold);
        PlayerPrefs.SetFloat("Exp", currentExp);
        PlayerPrefs.SetInt("MaxLevel", maxLevel);
    }

    public void SaveSkills()
    {
        ShieldBash.Save();
        BrutalAttack.Save();
        WideAttack.Save();
        PushingAttack.Save();
        BerserkMode.Save();

        PoisonTrap.Save();
        Invisibility.Save();
        Snare.Save();
        Multishot.Save();
        Decoy.Save();

        Thunderbolt.Save();
        PoisonMissile.Save();
        Glyph.Save();
        LifeDrain.Save();
        Sacrifice.Save();
    }


    public void TakeDamage(float damage)
    {
        if (GlyphObject.Instance != null && (Vector2)transform.position == Glyph.GlyphPosition)
            damage *= (100f - ((armor + Glyph.Armor) * defenceAmplifier)) / 100f;
        else
            damage *= (100f - (armor * defenceAmplifier)) / 100f;

        currentHealthPoints -= damage;
        HealthBar.fillAmount = currentHealthPoints / properties.HealthPoints;
        animator.SetTrigger("damage");

        if (currentHealthPoints <= 0)
        {
            this.enabled = false;
            Invoke("GameOver", 0.6f);
        }
    }


    private void Update()
    {
        if (IsUpdateValid())
        {
            if (GameManager.Instance.CurrentTurn == GameManager.Turns.Move)
                DoMoveTurn();
            else if (GameManager.Instance.CurrentTurn == GameManager.Turns.Actions)
                DoAttackTurn();
        }
    }

    public void UpdatePlayerHealth()
    {
        float fill = currentHealthPoints / properties.HealthPoints;

        HealthBar.fillAmount = fill;
    }

    public void UpdatePlayerMana()
    {
        float fill = currentManaPoints / properties.ManaPoints;

        ManaBar.fillAmount = fill;
    }

    public void UnblockUpdate()
    {
        IsUpdateBlocked = false;
    }

    public void UpdateSkill(int slot)
    {
        Skill skill = null;

        if (Instance.Class == PlayerClass.Warrior)
        {
            if (slot == 1)
                skill = ShieldBash;
            else if (slot == 2)
                skill = BrutalAttack;
            else if (slot == 3)
                skill = WideAttack;
            else if (slot == 4)
                skill = PushingAttack;
            else if (slot == 5)
                skill = BerserkMode;
        }
        else if (Class == PlayerClass.Rogue)
        {
            if (slot == 1)
                skill = PoisonTrap;
            else if (slot == 2)
                skill = Invisibility;
            else if (slot == 3)
                skill = Snare;
            else if (slot == 4)
                skill = Multishot;
            else if (slot == 5)
                skill = Decoy;
        }
        else if (Class == PlayerClass.Mage)
        {
            if (slot == 1)
                skill = Thunderbolt;
            else if (slot == 2)
                skill = PoisonMissile;
            else if (slot == 3)
                skill = Glyph;
            else if (slot == 4)
                skill = LifeDrain;
            else if (slot == 5)
                skill = Sacrifice;
        }

        skill.Upgrade();
        
        if(skill.Lvl == 5)
        {
            if (slot == 1)
                GameManager.Instance.UpgradeSkill1Button.interactable = false;
            else if (slot == 2)
                GameManager.Instance.UpgradeSkill2Button.interactable = false;
            else if (slot == 3)
                GameManager.Instance.UpgradeSkill3Button.interactable = false;
            else if (slot == 4)
                GameManager.Instance.UpgradeSkill4Button.interactable = false;
            else if (slot == 5)
                GameManager.Instance.UpgradeSkill5Button.interactable = false;
        }
    }
}