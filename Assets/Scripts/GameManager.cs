using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System;

public class GameManager : MonoBehaviour
{
    public enum Turns { Move, Actions };
    public enum Tabs { WarriorItems, RogueItems, MageItems, Items };

    private BoardManager board;
    private Slot[] chestSlots;
    private Slot[] inventorySlots;
    private Slot[] blacksmithSlots;
    private Weapon randomedWeapon;
    private Item gold;
    private Item randomedItem;
    private bool loaded = false;

    public static GameManager Instance = null;
    [HideInInspector]
    public bool InFight = false;
    [HideInInspector]
    public bool PlayerTurn = true;
    [HideInInspector]
    public Turns CurrentTurn = Turns.Move;
    [HideInInspector]
    public List<GenericEnemy> enemies;
    [HideInInspector]
    public bool MenuOn = false;
    [HideInInspector]
    public char[][] Obsacles = new char[100][];
    [HideInInspector]
    public Scene ActiveScene;
    [HideInInspector]
    public int level = 0;
    [HideInInspector]
    public Armor warriorSet;
    [HideInInspector]
    public Armor rogueSet;
    [HideInInspector]
    public Armor mageSet;
    [HideInInspector]
    public List<Weapon> warriorItems = new List<Weapon>();
    [HideInInspector]
    public List<Weapon> rogueItems = new List<Weapon>();
    [HideInInspector]
    public List<Weapon> mageItems = new List<Weapon>();
    [HideInInspector]
    public List<Item> Items = new List<Item>();
    [HideInInspector]
    public IOrderedEnumerable<KeyValuePair<GenericEnemy, float>> sortedEnemies;
    [HideInInspector]
    public Tabs currentTab;
    [HideInInspector]
    public int selectedItemSlot = -1;

    [HideInInspector]
    public bool NormalAttack = false;

    [HideInInspector]
    public bool ShieldBash = false;
    [HideInInspector]
    public bool BrutalAttack = false;
    [HideInInspector]
    public bool WideAttack = false;
    [HideInInspector]
    public bool PushingAttack = false;

    [HideInInspector]
    public bool Multishot = false;
    [HideInInspector]
    public bool DecoyIsUp = false;

    [HideInInspector]
    public bool Thunderbolt = false;
    [HideInInspector]
    public bool PoisonMissile = false;
    [HideInInspector]
    public bool LifeDrain = false;

    public Transform itemsHolder;
    public Weapon hand;

    public GameObject[] allWeapons;
    public GameObject[] allSets;
    public GameObject[] allItems;

    // ****** UI ******

    // **** Canvas ****
    public GameObject LoadingScreen;
    public GameObject InGameUI;
    public GameObject EndingScreen;
    public GameObject InGameMenu;
    public GameObject Chest;
    public GameObject ShowUI;

    // **** LoadingScreen ****

    public Text DungeonLevel;

    // **** InGameUI **** 

    public Button EndTurnButton;

    public Text TurnText;
    public Text MovesText;

    public Image AttackImage;

    public Text Skill1Text;
    public Text Skill2Text;
    public Text Skill3Text;

    public Image Skill1Image;
    public Image Skill2Image;
    public Image Skill3Image;

    public Text Item1Text;
    public Text Item2Text;

    public Image Item1Image;
    public Image Item2Image;

    // **** EndingScreen **** 

    public Text ReachedDungeonLevel;
    public Text ReachedLevel;
    public Text EnemiesKilled;
    public Text GoldGained;

    // **** InGameMenu ****

    // ** Panels **
    public GameObject PlayerPanel;
    public GameObject MapPanel;
    public GameObject InventoryPanel;
    public GameObject SkillsPanel;
    public GameObject BlacksmithPanel;
    public GameObject OptionsPanel;
    public GameObject ConfirmPanel;

    // ** ButtonsPanel **

    public Button MapButton;
    public Button BlacksmithButton;

    // ** PlayerPanel **

    // Stats
    public Text Class;
    public Text PlayerLevel;
    public Text Health;
    public Text Mana;
    public Text Strength;
    public Text Agility;
    public Text Intelligence;
    public Text Speed;
    public Text Damage;
    public Text Armor;
    public Text PlayerGold;
    public Text PlayerSkillPoints;

    // Experience
    public Image ExperienceBar;

    public Text CurrentExperience;
    public Text ExperienceToNextLevel;

    // Items
    public Image PlayerSlot1_Image;
    public Image PlayerSlot2_Image;

    public Text PlayerSlot1_Main;
    public Text PlayerSlot2_Main;

    public Text PlayerSlot1_Sub;
    public Text PlayerSlot2_Sub;

    // Classes
    public Button ChangeToWarrior;
    public Button ChangeToRogue;
    public Button ChangeToMage;

    // ** MapPanel **

    public Image map;

    // ** InventoryPanel **

    // Slots
    public Image InventorySlot1Image;
    public Image InventorySlot2Image;
    public Image InventorySlot3Image;
    public Image InventorySlot4Image;
    public Image InventorySlot5Image;
    public Image InventorySlot6Image;
    public Image InventorySlot7Image;
    public Image InventorySlot8Image;
    public Image InventorySlot9Image;
    public Image InventorySlot10Image;
    public Image InventorySlot11Image;
    public Image InventorySlot12Image;
    public Image InventorySlot13Image;
    public Image InventorySlot14Image;
    public Image InventorySlot15Image;
    public Image InventorySlot16Image;

    public Image InventorySlot1Button;
    public Image InventorySlot2Button;
    public Image InventorySlot3Button;
    public Image InventorySlot4Button;
    public Image InventorySlot5Button;
    public Image InventorySlot6Button;
    public Image InventorySlot7Button;
    public Image InventorySlot8Button;
    public Image InventorySlot9Button;
    public Image InventorySlot10Button;
    public Image InventorySlot11Button;
    public Image InventorySlot12Button;
    public Image InventorySlot13Button;
    public Image InventorySlot14Button;
    public Image InventorySlot15Button;
    public Image InventorySlot16Button;

    public Button equipButton;

    // Description
    public Text InventoryName;
    public Text InventoryDescription;
    public Text InventoryAmmount;

    // ** SkillsPanel **

    public Image MenuSkill1Image;
    public Image MenuSkill2Image;
    public Image MenuSkill3Image;
    public Image MenuSkill4Image;
    public Image MenuSkill5Image;

    public Text MenuSkill1Cost;
    public Text MenuSkill2Cost;
    public Text MenuSkill3Cost;
    public Text MenuSkill4Cost;
    public Text MenuSkill5Cost;

    public Text MenuSkill1NameBefore;
    public Text MenuSkill2NameBefore;
    public Text MenuSkill3NameBefore;
    public Text MenuSkill4NameBefore;
    public Text MenuSkill5NameBefore;

    public Text MenuSkill1NameAfter;
    public Text MenuSkill2NameAfter;
    public Text MenuSkill3NameAfter;
    public Text MenuSkill4NameAfter;
    public Text MenuSkill5NameAfter;

    public Text MenuSkill1DescriptionBefore;
    public Text MenuSkill2DescriptionBefore;
    public Text MenuSkill3DescriptionBefore;
    public Text MenuSkill4DescriptionBefore;
    public Text MenuSkill5DescriptionBefore;

    public Text MenuSkill1DescriptionAfter;
    public Text MenuSkill2DescriptionAfter;
    public Text MenuSkill3DescriptionAfter;
    public Text MenuSkill4DescriptionAfter;
    public Text MenuSkill5DescriptionAfter;

    public Button UpgradeSkill1Button;
    public Button UpgradeSkill2Button;
    public Button UpgradeSkill3Button;
    public Button UpgradeSkill4Button;
    public Button UpgradeSkill5Button;

    // ** BlacksmithPanel **

    // Slots
    public Image BlacksmithSlot1Image;
    public Image BlacksmithSlot2Image;
    public Image BlacksmithSlot3Image;
    public Image BlacksmithSlot4Image;
    public Image BlacksmithSlot5Image;
    public Image BlacksmithSlot6Image;
    public Image BlacksmithSlot7Image;
    public Image BlacksmithSlot8Image;
    public Image BlacksmithSlot9Image;
    public Image BlacksmithSlot10Image;
    public Image BlacksmithSlot11Image;
    public Image BlacksmithSlot12Image;
    public Image BlacksmithSlot13Image;
    public Image BlacksmithSlot14Image;
    public Image BlacksmithSlot15Image;
    public Image BlacksmithSlot16Image;

    public Image BlacksmithSlot1Button;
    public Image BlacksmithSlot2Button;
    public Image BlacksmithSlot3Button;
    public Image BlacksmithSlot4Button;
    public Image BlacksmithSlot5Button;
    public Image BlacksmithSlot6Button;
    public Image BlacksmithSlot7Button;
    public Image BlacksmithSlot8Button;
    public Image BlacksmithSlot9Button;
    public Image BlacksmithSlot10Button;
    public Image BlacksmithSlot11Button;
    public Image BlacksmithSlot12Button;
    public Image BlacksmithSlot13Button;
    public Image BlacksmithSlot14Button;
    public Image BlacksmithSlot15Button;
    public Image BlacksmithSlot16Button;

    // Item
    public Image UpgradeMenuItemImage;

    // Upgrade
    public Text UpgradeMenuItemName;
    public Text UpgradeMenuUpgradeCost;
    public Text UpgradeMenuItemStats;

    // Sell
    public Text UpgradeMenuSellCost;

    // ** OptionsPanel **

    public Slider music;
    public Slider sound;
    public Slider zoom;

    // **** Chest ****

    public Image ChestSlot1_Image;
    public Image ChestSlot2_Image;
    public Image ChestSlot3_Image;
    public Image ChestSlot4_Image;

    public Text ChestSlot1_Main;
    public Text ChestSlot2_Main;
    public Text ChestSlot3_Main;
    public Text ChestSlot4_Main;

    public Text ChestSlot1_Sub;
    public Text ChestSlot2_Sub;
    public Text ChestSlot3_Sub;
    public Text ChestSlot4_Sub;

    /**************************/

    public void Attack()
    {
        NormalAttack = true;

        ShieldBash = false;
        BrutalAttack = false;
        WideAttack = false;
        PushingAttack = false;

        Multishot = false;

        Thunderbolt = false;
        PoisonMissile = false;
        LifeDrain = false;
    }

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        Application.targetFrameRate = 30;
        enemies = new List<GenericEnemy>();
        board = GetComponent<BoardManager>();
        ConfirmPanel.SetActive(false);
        ActiveScene = SceneManager.GetActiveScene();
        DungeonLevel.text = "Loading...";

        for (int x = 0; x < 100; x++)
        {
            Obsacles[x] = new char[100];
        }

        for (int x = 0; x < 100; x++)
        {
            for (int y = 0; y < 100; y++)
            {
                Obsacles[x][y] = '-';
            }
        }

        chestSlots = new Slot[]
        {
            new Slot(ChestSlot1_Image, ChestSlot1_Main, ChestSlot1_Sub),
            new Slot(ChestSlot2_Image, ChestSlot2_Main, ChestSlot2_Sub),
            new Slot(ChestSlot3_Image, ChestSlot3_Main, ChestSlot3_Sub),
            new Slot(ChestSlot4_Image, ChestSlot4_Main, ChestSlot4_Sub)
        };

        inventorySlots = new Slot[]
        {
            new Slot(InventorySlot1Image, InventorySlot1Button),
            new Slot(InventorySlot2Image, InventorySlot2Button),
            new Slot(InventorySlot3Image, InventorySlot3Button),
            new Slot(InventorySlot4Image, InventorySlot4Button),
            new Slot(InventorySlot5Image, InventorySlot5Button),
            new Slot(InventorySlot6Image, InventorySlot6Button),
            new Slot(InventorySlot7Image, InventorySlot7Button),
            new Slot(InventorySlot8Image, InventorySlot8Button),
            new Slot(InventorySlot9Image, InventorySlot9Button),
            new Slot(InventorySlot10Image, InventorySlot10Button),
            new Slot(InventorySlot11Image, InventorySlot11Button),
            new Slot(InventorySlot12Image, InventorySlot12Button),
            new Slot(InventorySlot13Image, InventorySlot13Button),
            new Slot(InventorySlot14Image, InventorySlot14Button),
            new Slot(InventorySlot15Image, InventorySlot15Button),
            new Slot(InventorySlot16Image, InventorySlot16Button)
        };

        blacksmithSlots = new Slot[]
        {
            new Slot(BlacksmithSlot1Image, BlacksmithSlot1Button),
            new Slot(BlacksmithSlot2Image, BlacksmithSlot2Button),
            new Slot(BlacksmithSlot3Image, BlacksmithSlot3Button),
            new Slot(BlacksmithSlot4Image, BlacksmithSlot4Button),
            new Slot(BlacksmithSlot5Image, BlacksmithSlot5Button),
            new Slot(BlacksmithSlot6Image, BlacksmithSlot6Button),
            new Slot(BlacksmithSlot7Image, BlacksmithSlot7Button),
            new Slot(BlacksmithSlot8Image, BlacksmithSlot8Button),
            new Slot(BlacksmithSlot9Image, BlacksmithSlot9Button),
            new Slot(BlacksmithSlot10Image, BlacksmithSlot10Button),
            new Slot(BlacksmithSlot11Image, BlacksmithSlot11Button),
            new Slot(BlacksmithSlot12Image, BlacksmithSlot12Button),
            new Slot(BlacksmithSlot13Image, BlacksmithSlot13Button),
            new Slot(BlacksmithSlot14Image, BlacksmithSlot14Button),
            new Slot(BlacksmithSlot15Image, BlacksmithSlot15Button),
            new Slot(BlacksmithSlot16Image, BlacksmithSlot16Button)
         };
    }


    public void BlacksmithItemDetails(int slot)
    {
        selectedItemSlot = slot;

        if (currentTab == Tabs.WarriorItems)
        {
            if (slot >= warriorItems.Count) return;
            Sprite sprite = warriorItems[slot].image;

            UpgradeMenuItemImage.sprite = sprite;
            UpgradeMenuItemName.text = warriorItems[slot].WeponName.ToString();
            UpgradeMenuUpgradeCost.text = "Upgrade cost: " + warriorItems[slot].UpgradeCost();
            if (warriorItems[slot].armor == 0)
                UpgradeMenuItemStats.text = "Damage:" + warriorItems[slot].damage + " > " + (warriorItems[slot].damage + 10);
            else
                UpgradeMenuItemStats.text = "Damage:" + warriorItems[slot].damage + " > " + (warriorItems[slot].damage + 10) + "\n Armor: " + warriorItems[slot].armor + " > " + (warriorItems[slot].armor + 2);
            UpgradeMenuSellCost.text = "Sell for " + warriorItems[slot].SellCost();

            for (int i = 0; i < 16; i++)
                blacksmithSlots[i].Backgorund.color = Color.white;

            blacksmithSlots[slot].Backgorund.color = Color.yellow;
        }

        if (currentTab == Tabs.RogueItems)
        {
            if (slot >= rogueItems.Count) return;
            Sprite sprite = rogueItems[slot].image;

            UpgradeMenuItemImage.sprite = sprite;
            UpgradeMenuItemName.text = rogueItems[slot].WeponName.ToString();
            UpgradeMenuUpgradeCost.text = "Upgrade cost: " + rogueItems[slot].UpgradeCost();
            UpgradeMenuItemStats.text = "Damage:" + rogueItems[slot].damage + " > " + (rogueItems[slot].damage + 10);
            UpgradeMenuSellCost.text = "Sell for " + rogueItems[slot].SellCost();

            for (int i = 0; i < 16; i++)
                blacksmithSlots[i].Backgorund.color = Color.white;

            blacksmithSlots[slot].Backgorund.color = Color.yellow;
        }

        if (currentTab == Tabs.MageItems)
        {
            if (slot >= mageItems.Count) return;
            Sprite sprite = mageItems[slot].image;

            UpgradeMenuItemImage.sprite = sprite;
            UpgradeMenuItemName.text = mageItems[slot].WeponName.ToString();
            UpgradeMenuUpgradeCost.text = "Upgrade cost: " + mageItems[slot].UpgradeCost();
            UpgradeMenuItemStats.text = "Damage:" + mageItems[slot].damage + " > " + (mageItems[slot].damage + 10);
            UpgradeMenuSellCost.text = "Sell for " + mageItems[slot].SellCost();

            for (int i = 0; i < 16; i++)
                blacksmithSlots[i].Backgorund.color = Color.white;

            blacksmithSlots[slot].Backgorund.color = Color.yellow;
        }
    }


    public void ChangeTurnDelayed()
    {
        if (InFight)
            Player.Instance.IsUpdateBlocked = true;

        Invoke("ChangeTurn", 0.5f);
    }

    public void ChangeTurn()
    {
        if (InFight)
        {
            CurrentTurn = Turns.Move;

            if (PlayerTurn)
            {
                TurnText.text = "Enemys Turn";
                NormalAttack = true;
                MovesText.text = "";
                StartCoroutine(EnemiesTurn());
            }
            else
            {
                TurnText.text = "Player Turn";
                Player.Instance.currentMoves = Player.Instance.movesPerRound;
                MovesText.text = "Moves: " + Player.Instance.movesPerRound.ToString();

                if (Player.Instance.venomTime > 0)
                {
                    Player.Instance.venomTime--;
                    Player.Instance.TakeDamage(Player.Instance.venomDamage);
                }

                if(GlyphObject.Instance != null)
                    GlyphObject.Instance.decreaseLife();

                Player.Instance.ShieldBash.DecreaseCooldown(Skill1Image, Skill1Text);
                Player.Instance.BrutalAttack.DecreaseCooldown(Skill2Image, Skill2Text);
                Player.Instance.WideAttack.DecreaseCooldown(Skill1Image, Skill1Text);
                Player.Instance.PushingAttack.DecreaseCooldown(Skill2Image, Skill2Text);
                Player.Instance.BerserkMode.DecreaseCooldown(Skill3Image, Skill3Text);

                Player.Instance.PoisonTrap.DecreaseCooldown(Skill1Image, Skill1Text);
                Player.Instance.Invisibility.DecreaseCooldown(Skill2Image, Skill2Text);
                Player.Instance.Snare.DecreaseCooldown(Skill1Image, Skill1Text);
                Player.Instance.Multishot.DecreaseCooldown(Skill2Image, Skill2Text);
                Player.Instance.Decoy.DecreaseCooldown(Skill3Image, Skill3Text);

                Player.Instance.Thunderbolt.DecreaseCooldown(Skill1Image, Skill1Text);
                Player.Instance.PoisonMissile.DecreaseCooldown(Skill2Image, Skill2Text);
                Player.Instance.Glyph.DecreaseCooldown(Skill1Image, Skill1Text);
                Player.Instance.LifeDrain.DecreaseCooldown(Skill2Image, Skill2Text);
                Player.Instance.Sacrifice.DecreaseCooldown(Skill3Image, Skill3Text);

                Player.Instance.BerserkMode.DecreaseTurns();
                Player.Instance.Invisibility.DecreaseTurns();

                if(DecoyIsUp)
                    DecoyObject.Instance.DecreaseTurns();
            }

            if (!GenericEnemy.lastEnemy)
                PlayerTurn = !PlayerTurn;

            Player.Instance.UnblockUpdate();
        }
    }

    public void ClearInventory(string ui)
    {
        Slot[] slots;

        if (ui.Equals("inventory"))
            slots = inventorySlots;
        else
            slots = blacksmithSlots;

        foreach (Slot slot in slots)
            slot.ClearSlot();
    }

    public void CreateMap()
    {
        int mapWidth = board.boardWidth * 10;
        int mapHeight = board.boardHeight * 10;

        Texture2D mapTexture = new Texture2D(mapWidth, mapHeight);
        for (int i = 0; i < mapTexture.width; i = i + 10)
        {
            for (int j = 0; j < mapTexture.height; j = j + 10)
            {
                if (BoardManager.board[i / 10, j / 10] == BoardManager.BoardFields.Floor)
                {
                    for (int x = i; x < i + 10; x++)
                    {
                        for (int y = j; y < j + 10; y++)
                        {
                            //Floors
                            mapTexture.SetPixel(x, y, new Color(0.32f + Random.Range(-0.04f, 0.04f), 0.27f + Random.Range(-0.04f, 0.04f), 0.32f + Random.Range(-0.04f, 0.04f)));
                        }
                    }
                }
                else
                {
                    for (int x = i; x < i + 10; x++)
                    {
                        for (int y = j; y < j + 10; y++)
                        {
                            //Walls
                            mapTexture.SetPixel(x, y, new Color(0.13f + Random.Range(-0.04f, 0.04f), 0.07f + Random.Range(-0.04f, 0.04f), 0.06f + Random.Range(-0.04f, 0.04f)));
                        }
                    }
                }
            }
        }

        for (int x = (int)Player.Instance.transform.position.x * 10; x < (int)Player.Instance.transform.position.x * 10 + 10; x++)
        {
            for (int y = (int)Player.Instance.transform.position.y * 10; y < (int)Player.Instance.transform.position.y * 10 + 10; y++)
            {
                //Player
                mapTexture.SetPixel(x, y, new Color(0.9f, 1, 0));
            }
        }

        mapTexture.Apply();

        Sprite sprite = Sprite.Create(mapTexture, new Rect(0, 0, mapTexture.width, mapTexture.height), new Vector2(0.5f, 0.5f));

        map.sprite = sprite;
    }

    public void ChangeAlphaOfImage(Image image, float alpha)
    {
        Color color = image.color;
        color.a = alpha;
        image.color = color;
    }

    public void ChangeAlphaOfImage(SpriteRenderer image, float alpha)
    {
        Color color = image.color;
        color.a = alpha;
        image.color = color;
    }


    public void DrinkHealthPotion()
    {
        if (!InFight || PlayerTurn)
        {
            foreach (var item in Items)
            {
                if (item.itemName.Equals("Heal potion"))
                {
                    if (item.amount > 0)
                    {
                        item.amount--;
                        Player.Instance.currentHealthPoints += Player.Instance.properties.HealthPoints / 2;

                        if (Player.Instance.currentHealthPoints > Player.Instance.properties.HealthPoints)
                            Player.Instance.currentHealthPoints = Player.Instance.properties.HealthPoints;

                        Player.Instance.UpdatePlayerHealth();

                        if (item.amount == 0)
                            ChangeAlphaOfImage(Item2Image, 0.1f);

                        ChangeTurnDelayed();
                    }
                }
            }
        }
    }

    public void DrinkManaPotion()
    {
        if (!InFight || PlayerTurn)
        {
            foreach (var item in Items)
            {
                if (item.itemName.Equals("Mana potion"))
                {
                    if (item.amount > 0)
                    {
                        item.amount--;
                        Player.Instance.currentManaPoints += Player.Instance.properties.ManaPoints / 2;

                        if (Player.Instance.currentManaPoints > Player.Instance.properties.ManaPoints)
                            Player.Instance.currentManaPoints = Player.Instance.properties.ManaPoints;

                        Player.Instance.UpdatePlayerMana();

                        if (item.amount == 0)
                            ChangeAlphaOfImage(Item1Image, 0.1f);

                        ChangeTurnDelayed();
                    }
                }
            }
        }
    }


    public IEnumerator EnemiesTurn()
    {
        if(!Player.Instance.Invisible)
        {
            EndTurnButton.interactable = false;

            int times;
            Vector2 distance;
            float sqrDistance;
            Dictionary<GenericEnemy, float> tmp = new Dictionary<GenericEnemy, float>();

            foreach (var enemy in enemies)
            {
                if (enemy.enabled == false) continue;
                distance = enemy.transform.position - Player.Instance.transform.position;
                sqrDistance = distance.sqrMagnitude;

                tmp.Add(enemy, sqrDistance);
            }

            sortedEnemies = from pair in tmp
                            orderby pair.Value ascending
                            select pair;

            foreach (var enemy in sortedEnemies)
            {
                if(enemy.Key != null)
                {
                    if (enemy.Key.venomTime > 0)
                    {
                        enemy.Key.venomTime--;
                        enemy.Key.TakeDamage(enemy.Key.venomDamage);
                    }

                    if (enemy.Key.Stunned == 0)
                    {
                        if (enemy.Key.Immobilized == 0)
                        {
                            times = enemy.Key.Move();
                            yield return new WaitForSeconds(0.4f * times);
                        }
                        else
                            enemy.Key.Immobilized--;
                    }
                }
            }

            foreach (var enemy in sortedEnemies)
            {
                if (enemy.Key != null)
                {
                    if (enemy.Key.Stunned == 0)
                    {
                        times = enemy.Key.Attack();
                        yield return new WaitForSeconds(1.55f * times);
                    }
                    else
                        enemy.Key.Stunned--;
                }
            }

            EndTurnButton.interactable = true;
        }

        ChangeTurnDelayed();
    }

    public void Exit()
    {
        if (ActiveScene.name.Equals("Camp"))
            SaveGame(true);

        Application.Quit();
    }

    public void EndFight()
    {
        InFight = false;
        TurnText.text = "";
        MovesText.text = "";
        CurrentTurn = Turns.Move;
        PlayerTurn = true;
        Attack();
        EndTurnButton.interactable = false;

        Player.Instance.currentMoves = Player.Instance.movesPerRound;
        Player.Instance.RestartCooldowns();
        Player.Instance.UnblockUpdate();
    }


    private void HideLevelImage()
    {
        LoadingScreen.SetActive(false);

        InGameUI.SetActive(true);
    }


    public void InitGame(bool loaded)
    {
        LoadingScreen.SetActive(true);

        if (loaded)
        {
            loaded = false;
            DungeonLevel.text = "Level: " + (GameManager.Instance.level).ToString();
        }
        else
            level++;

        Invoke("HideLevelImage", 1.5f);
        Player.Instance.Invoke("SetCameraAndFloatingText", 1.5f);

        board.SetupScene();

        InFight = false;
        TurnText.text = "";
        MovesText.text = "";
        CurrentTurn = Turns.Move;
        SaveGame(true);
    }

    public void ItemDetails(int slot)
    {
        selectedItemSlot = slot;

        if (currentTab == Tabs.WarriorItems)
        {
            if (slot >= warriorItems.Count) return;

            InventoryName.text = warriorItems[slot].WeponName.ToString();

            if (warriorItems[slot].armor == 0)
                InventoryDescription.text = "Damage: " + warriorItems[slot].damage.ToString();
            else
                InventoryDescription.text = "Damage: " + warriorItems[slot].damage.ToString() + "\n Armor: " + warriorItems[slot].armor;

            if (warriorItems[slot].equiped)
                InventoryAmmount.text = "Current weapon";
            else
                InventoryAmmount.text = "";

            for (int i = 0; i < 16; i++)
                inventorySlots[i].Backgorund.color = Color.white;

            inventorySlots[slot].Backgorund.color = Color.yellow;
        }

        if (currentTab == Tabs.RogueItems)
        {
            if (slot >= rogueItems.Count) return;

            InventoryName.text = rogueItems[slot].WeponName.ToString();
            InventoryDescription.text = "Damage: " + rogueItems[slot].damage.ToString();

            if (rogueItems[slot].equiped)
                InventoryAmmount.text = "Current weapon";
            else
                InventoryAmmount.text = "";

            for (int i = 0; i < 16; i++)
                inventorySlots[i].Backgorund.color = Color.white;

            inventorySlots[slot].Backgorund.color = Color.yellow;
        }

        if (currentTab == Tabs.MageItems)
        {
            if (slot >= mageItems.Count) return;

            InventoryName.text = mageItems[slot].WeponName.ToString();
            InventoryDescription.text = "Damage: " + mageItems[slot].damage.ToString();

            if (mageItems[slot].equiped)
                InventoryAmmount.text = "Current weapon";
            else
                InventoryAmmount.text = "";

            for (int i = 0; i < 16; i++)
                inventorySlots[i].Backgorund.color = Color.white;

            inventorySlots[slot].Backgorund.color = Color.yellow;
        }

        if (currentTab == Tabs.Items)
        {
            if (slot >= Items.Count) return;
            InventoryName.text = Items[slot].itemName.ToString();
            InventoryDescription.text = Items[slot].description;
            InventoryAmmount.text = Items[slot].amount.ToString();

            for (int i = 0; i < 16; i++)
                inventorySlots[i].Backgorund.color = Color.white;

            inventorySlots[slot].Backgorund.color = Color.yellow;
        }
    }


    public void LoadGame()
    {
        level = PlayerPrefs.GetInt("Level");
        Player.Instance.LoadSkills();

        if (level == 0)
            SceneManager.LoadScene("Camp");
        else if (level > 0)
        {
            loaded = true;
            SceneManager.LoadScene("Anomaly");
        }

        Player.Instance.Class = (Player.PlayerClass)(Enum.Parse(typeof(Player.PlayerClass), PlayerPrefs.GetString("Class")));

        LoadOptions();
        LoadWarriorItems();
        LoadRogueItems();
        LoadMageItems();
        LoadItems();
        LoadSet();

        if (Player.Instance.weapon.WeponName.Equals("One handed sword"))
            Player.Instance.ChangeWeaponToOneHandedSword();
        else if (Player.Instance.weapon.WeponName.Equals("Two handed sword"))
            Player.Instance.ChangeWeaponToTwoHandedSword();
        else if (Player.Instance.weapon.WeponName.Equals("Daggers"))
            Player.Instance.ChangeWeaponToDagger();
        else if (Player.Instance.weapon.WeponName.Equals("Bow"))
            Player.Instance.ChangeWeaponToBow();
        else if (Player.Instance.weapon.WeponName.Equals("Staff"))
            Player.Instance.ChangeWeaponToStaff();
        else if (Player.Instance.weapon.WeponName.Equals("Rod"))
            Player.Instance.ChangeWeaponToRod();
     
        Player.Instance.Equip();
        Player.Instance.LoadStats();
        Player.Instance.LoadUiState();
        AttackImage.sprite = Player.Instance.weapon.image;
    }

    public void LoadOptions()
    {
        music.value = PlayerPrefs.GetFloat("MusicLevel");
        sound.value = PlayerPrefs.GetFloat("SoundLevel");
        zoom.value = PlayerPrefs.GetFloat("ZoomLevel");
    }

    public void LoadWarriorItems()
    {
        string id;
        Weapon weapon = null;
        int countWarrior = PlayerPrefs.GetInt("warriorItemsCount");

        for (int i = 0; i < countWarrior; i++)
        {
            id = "OneHandedSword" + i + "Damage";
            if (PlayerPrefs.HasKey(id))
            {
                GameObject OneHandedSword = Instantiate(allWeapons[3]);
                OneHandedSword.transform.SetParent(itemsHolder);
                weapon = (Weapon)OneHandedSword.GetComponent(typeof(Weapon)) as Weapon;

                weapon.damage = PlayerPrefs.GetInt(id);

                id = "OneHandedSword" + i + "Armor";
                weapon.armor = PlayerPrefs.GetInt(id);
            }

            id = "TwoHandedSword" + i + "Damage";
            if (PlayerPrefs.HasKey(id))
            {
                GameObject TwoHandedSword = Instantiate(allWeapons[2]);
                TwoHandedSword.transform.SetParent(itemsHolder);
                weapon = (Weapon)TwoHandedSword.GetComponent(typeof(Weapon)) as Weapon;

                weapon.damage = PlayerPrefs.GetInt(id);
            }

            warriorItems.Add(weapon);
        }

        if (PlayerPrefs.HasKey("CurrentWeaponWarrior"))
        {
            warriorItems[PlayerPrefs.GetInt("CurrentWeaponWarrior")].equiped = true;
            Player.Instance.weapon = warriorItems[PlayerPrefs.GetInt("CurrentWeaponWarrior")];
        }
    }

    public void LoadRogueItems()
    {
        string id;
        Weapon weapon = null;
        int countRogue = PlayerPrefs.GetInt("rogueItemsCount");

        for (int i = 0; i < countRogue; i++)
        {
            id = "Bow" + i + "Damage";
            if (PlayerPrefs.HasKey(id))
            {
                GameObject Bow = Instantiate(allWeapons[0]);
                Bow.transform.SetParent(itemsHolder);
                weapon = (Weapon)Bow.GetComponent(typeof(Weapon)) as Weapon;

                weapon.damage = PlayerPrefs.GetInt(id);
            }

            id = "Daggers" + i + "Damage";
            if (PlayerPrefs.HasKey(id))
            {
                GameObject Dagger = Instantiate(allWeapons[1]);
                Dagger.transform.SetParent(itemsHolder);
                weapon = (Weapon)Dagger.GetComponent(typeof(Weapon)) as Weapon;

                weapon.damage = PlayerPrefs.GetInt(id);
            }

            rogueItems.Add(weapon);
        }

        if (PlayerPrefs.HasKey("CurrentWeaponRogue"))
        {
            rogueItems[PlayerPrefs.GetInt("CurrentWeaponRogue")].equiped = true;
            Player.Instance.weapon = rogueItems[PlayerPrefs.GetInt("CurrentWeaponRogue")];
        }
    }

    public void LoadMageItems()
    {
        string id;
        Weapon weapon = null;
        int countMage = PlayerPrefs.GetInt("mageItemsCount");

        for (int i = 0; i < countMage; i++)
        {
            id = "Rod" + i + "Damage";
            if (PlayerPrefs.HasKey(id))
            {
                GameObject Rod = Instantiate(allWeapons[4]);
                Rod.transform.SetParent(itemsHolder);
                weapon = (Weapon)Rod.GetComponent(typeof(Weapon)) as Weapon;

                weapon.damage = PlayerPrefs.GetInt(id);
            }

            id = "Staff" + i + "Damage";
            if (PlayerPrefs.HasKey(id))
            {
                GameObject Staff = Instantiate(allWeapons[5]);
                Staff.transform.SetParent(itemsHolder);
                weapon = (Weapon)Staff.GetComponent(typeof(Weapon)) as Weapon;

                weapon.damage = PlayerPrefs.GetInt(id);
            }

            mageItems.Add(weapon);
        }

        if (PlayerPrefs.HasKey("CurrentWeaponMage"))
        {
            mageItems[PlayerPrefs.GetInt("CurrentWeaponMage")].equiped = true;
            Player.Instance.weapon = mageItems[PlayerPrefs.GetInt("CurrentWeaponMage")];
        }
    }

    public void LoadItems()
    {
        string id;

        id = "Heal potionAmmount";
        if (PlayerPrefs.HasKey(id))
        {
            GameObject Potion = Instantiate(allItems[1]);
            Potion.transform.SetParent(itemsHolder);
            Item item = (Item)Potion.GetComponent(typeof(Item)) as Item;

            item.amount = PlayerPrefs.GetInt(id);
            Items.Add(item);
        }

        id = "Mana potionAmmount";
        if (PlayerPrefs.HasKey(id))
        {
            GameObject Trap = Instantiate(allItems[2]);
            Trap.transform.SetParent(itemsHolder);
            Item item = (Item)Trap.GetComponent(typeof(Item)) as Item;

            item.amount = PlayerPrefs.GetInt(id);
            Items.Add(item);
        }
    }

    public void LoadSet()
    {
        warriorSet = Instantiate(allSets[1]).GetComponent(typeof(Armor)) as Armor;
        rogueSet = Instantiate(allSets[2]).GetComponent(typeof(Armor)) as Armor;
        mageSet = Instantiate(allSets[0]).GetComponent(typeof(Armor)) as Armor;

        warriorSet.transform.SetParent(itemsHolder);
        rogueSet.transform.SetParent(itemsHolder);
        mageSet.transform.SetParent(itemsHolder);

        if (Player.Instance.Class == Player.PlayerClass.Warrior)
            Player.Instance.set = warriorSet;
        else if (Player.Instance.Class == Player.PlayerClass.Rogue)
            Player.Instance.set = rogueSet;
        else if (Player.Instance.Class == Player.PlayerClass.Mage)
            Player.Instance.set = mageSet;  
    }

    public void LoadCamp()
    {
        SceneManager.LoadScene("Camp");
        EndingScreen.SetActive(false);
        ShowUI.SetActive(false);
        InGameUI.SetActive(true);
        Player.Instance.enabled = true;
    }


    private void OnLevelWasLoaded(int index)
    {
        ActiveScene = SceneManager.GetActiveScene();

        if (index != 0)
            HideLevelImage();

        if (index == 1)
        {
            StartCoroutine(SavePeriodically());

            EndTurnButton.interactable = false;
            MapButton.interactable = false;
            BlacksmithButton.interactable = true;
            ChangeToWarrior.interactable = true;
            ChangeToRogue.interactable = true;
            ChangeToMage.interactable = true;
            UpgradeSkill1Button.interactable = true;
            UpgradeSkill2Button.interactable = true;
            UpgradeSkill3Button.interactable = true;
            UpgradeSkill4Button.interactable = true;
            UpgradeSkill5Button.interactable = true;
        }
        else if (index == 2)
        {
            InitGame(loaded);

            EndTurnButton.interactable = true;
            MapButton.interactable = true;
            BlacksmithButton.interactable = false;
            ChangeToWarrior.interactable = false;
            ChangeToRogue.interactable = false;
            ChangeToMage.interactable = false;

            loaded = false;
        }
    }


    private void NewGame()
    {
        Weapon oneHandedSword = Instantiate(allWeapons[3]).GetComponent(typeof(Weapon)) as Weapon;
        Weapon twoHandedSword = Instantiate(allWeapons[2]).GetComponent(typeof(Weapon)) as Weapon;
        Weapon daggers = Instantiate(allWeapons[1]).GetComponent(typeof(Weapon)) as Weapon;
        Weapon bow = Instantiate(allWeapons[0]).GetComponent(typeof(Weapon)) as Weapon;
        Weapon staff = Instantiate(allWeapons[5]).GetComponent(typeof(Weapon)) as Weapon;
        Weapon rod = Instantiate(allWeapons[4]).GetComponent(typeof(Weapon)) as Weapon;

        oneHandedSword.transform.SetParent(itemsHolder);
        twoHandedSword.transform.SetParent(itemsHolder);
        daggers.transform.SetParent(itemsHolder);
        bow.transform.SetParent(itemsHolder);
        staff.transform.SetParent(itemsHolder);
        rod.transform.SetParent(itemsHolder);

        warriorSet = Instantiate(allSets[1]).GetComponent(typeof(Armor)) as Armor;
        rogueSet = Instantiate(allSets[2]).GetComponent(typeof(Armor)) as Armor;
        mageSet = Instantiate(allSets[0]).GetComponent(typeof(Armor)) as Armor;

        warriorSet.firStyleActive = true;
        rogueSet.firStyleActive = true;
        mageSet.firStyleActive = true;

        warriorSet.transform.SetParent(itemsHolder);
        rogueSet.transform.SetParent(itemsHolder);
        mageSet.transform.SetParent(itemsHolder);

        Player.Instance.weapon = oneHandedSword;
        Player.Instance.set = warriorSet;
        Player.Instance.Equip();

        oneHandedSword.equiped = true;
        warriorItems.Add(oneHandedSword);
        warriorItems.Add(twoHandedSword);
        rogueItems.Add(daggers);
        rogueItems.Add(bow);
        mageItems.Add(staff);
        mageItems.Add(rod);

        Item healPotion = Instantiate(allItems[1]).GetComponent(typeof(Item)) as Item;
        Item manaPotion = Instantiate(allItems[2]).GetComponent(typeof(Item)) as Item;

        healPotion.amount++;
        manaPotion.amount++;

        Items.Add(healPotion);
        Items.Add(manaPotion);

        healPotion.transform.SetParent(itemsHolder);
        manaPotion.transform.SetParent(itemsHolder);

        Player.Instance.Level = 1;

        Player.Instance.ShieldBash.Restart();
        Player.Instance.BrutalAttack.Restart();
        Player.Instance.WideAttack.Restart();
        Player.Instance.PushingAttack.Restart();
        Player.Instance.BerserkMode.Restart();

        Player.Instance.PoisonTrap.Restart();
        Player.Instance.Invisibility.Restart();
        Player.Instance.Snare.Restart();
        Player.Instance.Multishot.Restart();
        Player.Instance.Decoy.Restart();

        Player.Instance.Thunderbolt.Restart();
        Player.Instance.PoisonMissile.Restart();
        Player.Instance.Glyph.Restart();
        Player.Instance.LifeDrain.Restart();
        Player.Instance.Sacrifice.Restart();

        Player.Instance.skillPoints = 1;
        Player.Instance.SetStats();

        SceneManager.LoadScene("Camp");
        Player.Instance.GainExp(0);
    }


    public void RandomItems()
    {
        int count = 0;

        if (RandomWeapon()) count++;
        if (RandomGold())   count++;
        if (RandomItem())   count++;

        if (count == 0) RandomItems();
    }

    public bool RandomWeapon()
    {
        int rand;
        bool randomed = false;
        Sprite sprite;

        rand = Random.Range(1, 101);
        if (rand >= 1 && rand <= 40)
        {
            randomed = true;

            GameObject random = Instantiate(allWeapons[Random.Range(0, allWeapons.Length)]);
            random.transform.SetParent(itemsHolder);
            randomedWeapon = random.GetComponent(typeof(Weapon)) as Weapon;
            rand = Random.Range(1, 101);

            if (randomedWeapon.WeponName.Equals("One handed sword"))
            {
                if (rand >= 1 && rand <= 10)
                {
                    randomedWeapon.damage = Random.Range(30, 39);
                    randomedWeapon.armor = Random.Range(10, 14);
                }
                else if (rand >= 11 && rand <= 50)
                {
                    randomedWeapon.damage = Random.Range(20, 29);
                    randomedWeapon.armor = Random.Range(6, 9);
                }
                else
                {
                    randomedWeapon.damage = Random.Range(10, 19);
                    randomedWeapon.armor = Random.Range(2, 5);
                }
            }
            else if (randomedWeapon.WeponName.Equals("Two handed sword"))
            {
                if (rand >= 1 && rand <= 10)
                    randomedWeapon.damage = Random.Range(55, 75);
                else if (rand >= 11 && rand <= 50)
                    randomedWeapon.damage = Random.Range(40, 54);
                else
                    randomedWeapon.damage = Random.Range(25, 39);
            }
            else if (randomedWeapon.WeponName.Equals("Dagger"))
            {
                if (rand >= 1 && rand <= 10)
                    randomedWeapon.damage = Random.Range(55, 75);
                else if (rand >= 11 && rand <= 50)
                    randomedWeapon.damage = Random.Range(40, 54);
                else
                    randomedWeapon.damage = Random.Range(25, 39);
            }
            else if (randomedWeapon.WeponName.Equals("Bow"))
            {
                if (rand >= 1 && rand <= 10)
                    randomedWeapon.damage = Random.Range(45, 60);
                else if (rand >= 11 && rand <= 50)
                    randomedWeapon.damage = Random.Range(35, 44);
                else
                    randomedWeapon.damage = Random.Range(25, 34);
            }
            else if (randomedWeapon.WeponName.Equals("Staff"))
            {
                if (rand >= 1 && rand <= 10)
                    randomedWeapon.damage = Random.Range(55, 75);
                else if (rand >= 11 && rand <= 50)
                    randomedWeapon.damage = Random.Range(40, 54);
                else
                    randomedWeapon.damage = Random.Range(25, 39);
            }
            else if (randomedWeapon.WeponName.Equals("Rod"))
            {
                if (rand >= 1 && rand <= 10)
                    randomedWeapon.damage = Random.Range(45, 60);
                else if (rand >= 11 && rand <= 50)
                    randomedWeapon.damage = Random.Range(35, 44);
                else
                    randomedWeapon.damage = Random.Range(25, 34);
            }

            sprite = randomedWeapon.image;

            foreach (Slot slot in chestSlots)
            {
                if (slot.state == Slot.State.Empty)
                {
                    slot.Image.sprite = sprite;
                    slot.mainText.text = randomedWeapon.WeponName.ToString();

                    if (randomedWeapon.armor != 0)
                        slot.subText.text = "Damage: " + randomedWeapon.damage + "\n Armor: " + randomedWeapon.armor;
                    else
                        slot.subText.text = "Damage: " + randomedWeapon.damage;

                    slot.state = Slot.State.Taken;
                    break;
                }
            }
        }

        return randomed;
    }

    public bool RandomGold()
    {
        int rand;
        bool randomed = false;
        Sprite sprite;

        rand = Random.Range(1, 101);
        if (rand >= 1 && rand <= 60)
        {
            randomed = true;

            GameObject random = Instantiate(allItems[0]);
            random.transform.SetParent(itemsHolder);
            gold = random.GetComponent(typeof(Item)) as Item;

            gold.amount = Random.Range(1, 101);
            Player.Instance.EndGameInfo.GoldGained += gold.amount;
            sprite = gold.image;

            foreach (Slot slot in chestSlots)
            {
                if (slot.state == Slot.State.Empty)
                {
                    slot.Image.sprite = sprite;
                    slot.mainText.text = gold.itemName.ToString();
                    slot.subText.text = "Ammount: " + gold.amount;
                    slot.state = Slot.State.Taken;

                    break;
                }
            }
        }

        return randomed;
    }

    public bool RandomItem()
    {
        int rand;
        bool randomed = false;
        Sprite sprite;

        rand = Random.Range(1, 101);
        if (rand >= 1 && rand <= 50)
        {
            randomed = true;

            GameObject random = Instantiate(allItems[Random.Range(1, 3)]);
            random.transform.SetParent(itemsHolder);
            randomedItem = random.GetComponent(typeof(Item)) as Item;

            randomedItem.amount = 1;
            sprite = randomedItem.image;

            foreach (Slot slot in chestSlots)
            {
                if (slot.state == Slot.State.Empty)
                {
                    slot.Image.sprite = sprite;
                    slot.mainText.text = randomedItem.itemName.ToString();
                    slot.subText.text = "Ammount: " + randomedItem.amount;
                    slot.state = Slot.State.Taken;

                    break;
                }
            }
        }

        return randomed;
    }


    public void Start()
    {
        if (PlayerPrefs.HasKey("PreviousGame1"))
            LoadGame();
        else
            NewGame();
    }

    public void SaveGame(bool saveImmediately)
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetInt("PreviousGame1", 1);
        PlayerPrefs.SetInt("Level", level);
        Player.Instance.SaveStats();
        Player.Instance.SaveSkills();

        SaveOptions();
        SaveWarriorItems();
        SaveRogueItems();
        SaveMageItems();
        SaveItems();

        if (saveImmediately)
            PlayerPrefs.Save();
    }

    public void SaveOptions()
    {
        PlayerPrefs.SetFloat("MusicLevel", music.value);
        PlayerPrefs.SetFloat("SoundLevel", sound.value);
        PlayerPrefs.SetFloat("ZoomLevel", zoom.value);
    }

    public void SaveWarriorItems()
    {
        string id;
        int value;

        PlayerPrefs.SetInt("warriorItemsCount", warriorItems.Count);

        for (int i = 0; i < warriorItems.Count; i++)
        {
            if (warriorItems[i].WeponName.Equals("One handed sword"))
            {
                id = "OneHandedSword" + i + "Damage";
                value = warriorItems[i].damage;
                PlayerPrefs.SetInt(id, value);

                id = "OneHandedSword" + i + "Armor";
                value = warriorItems[i].armor;
                PlayerPrefs.SetInt(id, value);

                if (warriorItems[i].equiped)
                    PlayerPrefs.SetInt("CurrentWeaponWarrior", i);
            }
            else
            {
                id = "TwoHandedSword" + i + "Damage";
                value = warriorItems[i].damage;
                PlayerPrefs.SetInt(id, value);

                if (warriorItems[i].equiped)
                    PlayerPrefs.SetInt("CurrentWeaponWarrior", i);
            }
        }
    }

    public void SaveRogueItems()
    {
        string id;
        int value;

        PlayerPrefs.SetInt("rogueItemsCount", rogueItems.Count);

        for (int i = 0; i < rogueItems.Count; i++)
        {
            id = rogueItems[i].WeponName.ToString() + i + "Damage";
            value = rogueItems[i].damage;
            PlayerPrefs.SetInt(id, value);

            if (rogueItems[i].equiped)
                PlayerPrefs.SetInt("CurrentWeaponRogue", i);
        }
    }

    public void SaveMageItems()
    {
        string id;
        int value;

        PlayerPrefs.SetInt("mageItemsCount", mageItems.Count);

        for (int i = 0; i < mageItems.Count; i++)
        {
            id = mageItems[i].WeponName.ToString() + i + "Damage";
            value = mageItems[i].damage;
            PlayerPrefs.SetInt(id, value);

            if (mageItems[i].equiped)
                PlayerPrefs.SetInt("CurrentWeaponMage", i);
        }
    }

    public void SaveItems()
    {
        string id;
        int value;

        PlayerPrefs.SetInt("ItemsCount", Items.Count);

        for (int i = 0; i < Items.Count; i++)
        {
            id = Items[i].itemName.ToString() + "Ammount";
            value = Items[i].amount;
            PlayerPrefs.SetInt(id, value);
        }
    }

    public void ShowWarriorItems(string ui)
    {
        currentTab = Tabs.WarriorItems;
        Slot[] slots; 

        if (ui.Equals("inventory"))
        {
            slots = inventorySlots;
            equipButton.interactable = true;
        }       
        else
            slots = blacksmithSlots;

        ClearInventory(ui);

        if (Player.Instance.Class == Player.PlayerClass.Warrior)
        { 
            for (int j = 0; j < warriorItems.Count; j++)
                if (warriorItems[j].equiped)
                    selectedItemSlot = j;
        }
        else if (warriorItems.Count > 0)
            selectedItemSlot = 0;

        if (ui.Equals("inventory"))
        {
            InventoryAmmount.text = "";
            inventorySlots[selectedItemSlot].Backgorund.color = Color.yellow;
            ItemDetails(selectedItemSlot);
        }
        else
        {
            blacksmithSlots[selectedItemSlot].Backgorund.color = Color.yellow;
            BlacksmithItemDetails(selectedItemSlot);         
        }

        for (int i = 0; i < warriorItems.Count; i++)
        {
            if (slots[i].state == Slot.State.Empty)
            {
                slots[i].Image.sprite = warriorItems[i].image;
                slots[i].state = Slot.State.Taken;
            }
        }
    }

    public void ShowRogueItems(string ui)
    {
        currentTab = Tabs.RogueItems;
        Slot[] slots;

        if (ui.Equals("inventory"))
        {
            slots = inventorySlots;
            equipButton.interactable = true;
        }
        else
            slots = blacksmithSlots;

        ClearInventory(ui);

        if (Player.Instance.Class == Player.PlayerClass.Rogue)
        {
            for (int j = 0; j < rogueItems.Count; j++)
                if (rogueItems[j].equiped)
                    selectedItemSlot = j;
        }
        else if (rogueItems.Count > 0)
            selectedItemSlot = 0;

        if (ui.Equals("inventory"))
        {
            InventoryAmmount.text = "";
            inventorySlots[selectedItemSlot].Backgorund.color = Color.yellow;
            ItemDetails(selectedItemSlot);
        }
        else
        {
            blacksmithSlots[selectedItemSlot].Backgorund.color = Color.yellow;
            BlacksmithItemDetails(selectedItemSlot);
        }

        for (int i = 0; i < rogueItems.Count; i++)
        {
            if (slots[i].state == Slot.State.Empty)
            {
                slots[i].Image.sprite = rogueItems[i].image;
                slots[i].state = Slot.State.Taken;
            }
        }
    }

    public void ShowMageItems(string ui)
    {
        currentTab = Tabs.MageItems;
        Slot[] slots;

        if (ui.Equals("inventory"))
        {
            slots = inventorySlots;
            equipButton.interactable = true;
        }
        else
            slots = blacksmithSlots;

        ClearInventory(ui);

        if (Player.Instance.Class == Player.PlayerClass.Mage)
        {
            for (int j = 0; j < mageItems.Count; j++)
                if (mageItems[j].equiped)
                    selectedItemSlot = j;
        }
        else if (mageItems.Count > 0)
            selectedItemSlot = 0;

        if (ui.Equals("inventory"))
        {
            InventoryAmmount.text = "";
            inventorySlots[selectedItemSlot].Backgorund.color = Color.yellow;
            ItemDetails(selectedItemSlot);
        }
        else
        {
            blacksmithSlots[selectedItemSlot].Backgorund.color = Color.yellow;
            BlacksmithItemDetails(selectedItemSlot);
        }

        for (int i = 0; i < mageItems.Count; i++)
        {
            if (slots[i].state == Slot.State.Empty)
            {
                slots[i].Image.sprite = mageItems[i].image;
                slots[i].state = Slot.State.Taken;
            }
        }
    }

    public void ShowItems(string ui)
    {
        currentTab = Tabs.Items;
        Slot[] slots;

        if (ui.Equals("inventory"))
        {
            slots = inventorySlots;
            equipButton.interactable = false;
        }
        else
            slots = blacksmithSlots;

        ClearInventory(ui);

        for (int i = 0; i < Items.Count; i++)
        {
            if (slots[i].state == Slot.State.Empty)
            {
                slots[i].Image.sprite = Items[i].image;
                slots[i].state = Slot.State.Taken;
            }
        }
    }

    public void ShowPlayer()
    {
        Sprite sprite;

        PlayerPanel.SetActive(true);
        MapPanel.SetActive(false);
        InventoryPanel.SetActive(false);
        SkillsPanel.SetActive(false);
        BlacksmithPanel.SetActive(false);
        OptionsPanel.SetActive(false);

        Class.text = Player.Instance.Class.ToString();
        PlayerLevel.text = Player.Instance.Level.ToString();
        PlayerGold.text = Player.Instance.gold.ToString();
        Health.text = Player.Instance.properties.HealthPoints.ToString();
        Mana.text = Player.Instance.properties.ManaPoints.ToString();
        Strength.text = Player.Instance.properties.StrengthPoints.ToString();
        Agility.text = Player.Instance.properties.AgilityPoints.ToString();
        Intelligence.text = Player.Instance.properties.IntelligencePoints.ToString();
        Speed.text = Player.Instance.movesPerRound.ToString();
        Damage.text = Player.Instance.damage.ToString();
        Armor.text = Player.Instance.armor.ToString();
        PlayerSkillPoints.text = Player.Instance.skillPoints.ToString();

        sprite = Player.Instance.weapon.image;
        PlayerSlot1_Image.sprite = sprite;
        PlayerSlot1_Main.text = Player.Instance.weapon.WeponName.ToString();
        PlayerSlot1_Sub.text = "Damage: " + Player.Instance.weapon.damage;

        sprite = Player.Instance.set.image;
        PlayerSlot2_Image.sprite = sprite;
        PlayerSlot2_Main.text = Player.Instance.set.ArmorName;
        PlayerSlot2_Sub.text = "Armor: " + Player.Instance.set.armor;
    }

    public void ShowMap()
    {
        CreateMap();

        PlayerPanel.SetActive(false);
        MapPanel.SetActive(true);
        InventoryPanel.SetActive(false);
        SkillsPanel.SetActive(false);
        BlacksmithPanel.SetActive(false);
        OptionsPanel.SetActive(false);
    }

    public void ShowInventory()
    {
        PlayerPanel.SetActive(false);
        MapPanel.SetActive(false);
        InventoryPanel.SetActive(true);
        SkillsPanel.SetActive(false);
        BlacksmithPanel.SetActive(false);
        OptionsPanel.SetActive(false);

        for (int i = 0; i < 16; i++)
            inventorySlots[i].Backgorund.color = Color.white;

        if (Player.Instance.Class == Player.PlayerClass.Warrior)
            ShowWarriorItems("inventory");
        else if (Player.Instance.Class == Player.PlayerClass.Rogue)
            ShowRogueItems("inventory");
        else if (Player.Instance.Class == Player.PlayerClass.Mage)
            ShowMageItems("inventory");
    }

    public void ShowSkills()
    {
        PlayerPanel.SetActive(false);
        MapPanel.SetActive(false);
        InventoryPanel.SetActive(false);
        SkillsPanel.SetActive(true);
        BlacksmithPanel.SetActive(false);
        OptionsPanel.SetActive(false);

        if (Player.Instance.Class == Player.PlayerClass.Warrior)
            ShowWarriorSkills();
        else if (Player.Instance.Class == Player.PlayerClass.Rogue)
            ShowRogueSkills();
        else if (Player.Instance.Class == Player.PlayerClass.Mage)
            ShowMageSkills();
    }

    private void ShowWarriorSkills()
    {
        ShieldBash skill1 = Player.Instance.ShieldBash;

        MenuSkill1Image.sprite = skill1.Image;
        MenuSkill1NameBefore.text = skill1.Name + " " + skill1.Lvl;
        MenuSkill1DescriptionBefore.text = "Cooldown: " + skill1.Cooldown + "\n ManaCost: " + skill1.ManaCost + "\n Damage: " + skill1.Damage + "\n Stun duration: " + skill1.StunDuration;

        if (skill1.Lvl < 5)
        {
            MenuSkill1Cost.text = "Uprgrade cost " + skill1.UpgradeCost();
            MenuSkill1NameAfter.text = skill1.Name + " " + (skill1.Lvl + 1);
            MenuSkill1DescriptionAfter.text = "Cooldown: " + (skill1.Cooldown - ((skill1.Lvl + 1) % 2)) + "\n ManaCost: " + (skill1.ManaCost - 2) + "\n Damage: " + (skill1.Damage + 10) + "\n Stun duration: " + (skill1.StunDuration - Mathf.Max(0, skill1.UpgradeCost() - 2));
        }
        else
        {
            MenuSkill1Cost.text = "Max level";
            MenuSkill1NameAfter.text = "";
            MenuSkill1DescriptionAfter.text = "";
        }


        BrutalAttack skill2 = Player.Instance.BrutalAttack;

        MenuSkill2Image.sprite = skill2.Image;
        MenuSkill2NameBefore.text = skill2.Name + " " + skill2.Lvl;
        MenuSkill2DescriptionBefore.text = "Cooldown: " + skill2.Cooldown + "\n ManaCost: " + skill2.ManaCost + "\n Damage: " + skill2.Damage;

        if (skill2.Lvl < 5)
        {
            MenuSkill2Cost.text = "Uprgrade cost " + skill2.UpgradeCost();
            MenuSkill2NameAfter.text = skill2.Name + " " + (skill2.Lvl + 1);
            MenuSkill2DescriptionAfter.text = "Cooldown: " + (skill2.Cooldown - ((skill2.Lvl + 1) % 2)) + "\n ManaCost: " + (skill2.ManaCost - 1) + "\n Damage: " + (skill2.Damage + 30);
        }
        else
        {
            MenuSkill2Cost.text = "Max level";
            MenuSkill2NameAfter.text = "";
            MenuSkill2DescriptionAfter.text = "";
        }


        WideAttack skill3 = Player.Instance.WideAttack;

        MenuSkill3Image.sprite = skill3.Image;
        MenuSkill3NameBefore.text = skill3.Name + " " + skill3.Lvl;
        MenuSkill3DescriptionBefore.text = "Cooldown: " + skill3.Cooldown + "\n ManaCost: " + skill3.ManaCost + "\n Damage: " + (skill3.Damage + Player.Instance.damage);

        if (skill3.Lvl < 5)
        {
            MenuSkill3Cost.text = "Uprgrade cost " + skill3.UpgradeCost();
            MenuSkill3NameAfter.text = skill3.Name + " " + (skill3.Lvl + 1);
            MenuSkill3DescriptionAfter.text = "Cooldown: " + (skill3.Cooldown - ((skill3.Lvl + 1) % 2)) + "\n ManaCost: " + (skill3.ManaCost - 2) + "\n Damage: " + (skill3.Damage + 10 + Player.Instance.damage);
        }
        else
        {
            MenuSkill3Cost.text = "Max level";
            MenuSkill3NameAfter.text = "";
            MenuSkill3DescriptionAfter.text = "";
        }


        PushingAttack skill4 = Player.Instance.PushingAttack;

        MenuSkill4Image.sprite = skill4.Image;
        MenuSkill4NameBefore.text = skill4.Name + " " + skill4.Lvl;
        MenuSkill4DescriptionBefore.text = "Cooldown: " + skill4.Cooldown + "\n ManaCost: " + skill4.ManaCost + "\n Damage: " + (skill4.Damage + Player.Instance.damage);

        if (skill4.Lvl < 5)
        {
            MenuSkill4Cost.text = "Uprgrade cost " + skill4.UpgradeCost();
            MenuSkill4NameAfter.text = skill4.Name + " " + (skill4.Lvl + 1);
            MenuSkill4DescriptionAfter.text = "Cooldown: " + (skill4.Cooldown - ((skill4.Lvl + 1) % 2)) + "\n ManaCost: " + (skill4.ManaCost - 1) + "\n Damage: " + (skill4.Damage + 15 + Player.Instance.damage);
        }
        else
        {
            MenuSkill4Cost.text = "Max level";
            MenuSkill4NameAfter.text = "";
            MenuSkill4DescriptionAfter.text = "";
        }


        BerserkMode skill5 = Player.Instance.BerserkMode;

        MenuSkill5Image.sprite = skill5.Image;
        MenuSkill5NameBefore.text = skill5.Name + " " + skill5.Lvl;
        MenuSkill5DescriptionBefore.text = "Cooldown: " + skill5.Cooldown + "\n ManaCost: " + skill5.ManaCost + "\n Damage and defence amplifier: " + skill5.damageAmplifier + "\n Lasts tunrs: " + skill5.Turns;

        if (skill5.Lvl < 5)
        {
            MenuSkill5Cost.text = "Uprgrade cost " + skill5.UpgradeCost();
            MenuSkill5NameAfter.text = skill5.Name + " " + (skill5.Lvl + 1);
            MenuSkill5DescriptionAfter.text = "Cooldown: " + (skill5.Cooldown - ((skill5.Lvl + 1) % 2)) + "\n ManaCost: " + (skill5.ManaCost - 1) + "\n Damage and defence amplifier: " + (skill5.damageAmplifier + 0.2f) + "\n Lasts tunrs: " + (skill5.Turns + ((skill5.Lvl + 1) % 2));
        }
        else
        {
            MenuSkill5Cost.text = "Max level";
            MenuSkill5NameAfter.text = "";
            MenuSkill5DescriptionAfter.text = "";
        }
    }

    private void ShowRogueSkills()
    {
        PoisonTrap skill1 = Player.Instance.PoisonTrap;

        MenuSkill1Image.sprite = skill1.Image;
        MenuSkill1NameBefore.text = skill1.Name + " " + skill1.Lvl;
        MenuSkill1DescriptionBefore.text = "Cooldown: " + skill1.Cooldown + "\n ManaCost: " + skill1.ManaCost + "\n Damage: " + skill1.Damage + "\n Venom damage: " + skill1.VenomDamage + "\n Venom time: " + skill1.VenomTime;

        if (skill1.Lvl < 5)
        {
            MenuSkill1Cost.text = "Uprgrade cost " + skill1.UpgradeCost();
            MenuSkill1NameAfter.text = skill1.Name + " " + (skill1.Lvl + 1);
            MenuSkill1DescriptionAfter.text = "Cooldown: " + (skill1.Cooldown - ((skill1.Lvl + 1) % 2)) + "\n ManaCost: " + (skill1.ManaCost - 2) + "\n Damage: " + (skill1.Damage + 20) + "\n  Venom damage: " + (skill1.VenomDamage + 20) + "\n Venom time: " + (skill1.VenomTime - ((skill1.Lvl + 1) % 2));
        }
        else
        {
            MenuSkill1Cost.text = "Max level";
            MenuSkill1NameAfter.text = "";
            MenuSkill1DescriptionAfter.text = "";
        }


        Invisibility skill2 = Player.Instance.Invisibility;

        MenuSkill2Image.sprite = skill2.Image;
        MenuSkill2NameBefore.text = skill2.Name + " " + skill2.Lvl;
        MenuSkill2DescriptionBefore.text = "Cooldown: " + skill2.Cooldown + "\n ManaCost: " + skill2.ManaCost + "\n Damage amplifier: " + skill2.damageAmplifier + "\n Lasts tunrs: " + skill2.Turns;

        if (skill2.Lvl < 5)
        {
            MenuSkill2Cost.text = "Uprgrade cost " + skill2.UpgradeCost();
            MenuSkill2NameAfter.text = skill2.Name + " " + (skill2.Lvl + 1);
            MenuSkill2DescriptionAfter.text = "Cooldown: " + (skill2.Cooldown - ((skill2.Lvl + 1) % 2)) + "\n ManaCost: " + (skill2.ManaCost - 2) + "\n Damage amplifier: " + (skill2.damageAmplifier + 0.1f) + "\n Lasts tunrs: " + (skill2.Turns + ((skill2.Lvl + 1) % 2));
        }
        else
        {
            MenuSkill2Cost.text = "Max level";
            MenuSkill2NameAfter.text = "";
            MenuSkill2DescriptionAfter.text = "";
        }


        Snare skill3 = Player.Instance.Snare;

        MenuSkill3Image.sprite = skill3.Image;
        MenuSkill3NameBefore.text = skill3.Name + " " + skill3.Lvl;
        MenuSkill3DescriptionBefore.text = "Cooldown: " + skill3.Cooldown + "\n ManaCost: " + skill3.ManaCost + "\n Damage: " + skill3.Damage + "\n Immobilize: " + skill3.Immobilize;

        if (skill3.Lvl < 5)
        {
            MenuSkill3Cost.text = "Uprgrade cost " + skill3.UpgradeCost();
            MenuSkill3NameAfter.text = skill3.Name + " " + (skill3.Lvl + 1);
            MenuSkill3DescriptionAfter.text = "Cooldown: " + (skill3.Cooldown - ((skill3.Lvl + 1) % 2)) + "\n ManaCost: " + (skill3.ManaCost - 2) + "\n Damage: " + (skill3.Damage + 20) + "\n Immobilize: " + (skill3.Immobilize + ((skill3.Lvl + 1) % 2));
        }
        else
        {
            MenuSkill3Cost.text = "Max level";
            MenuSkill3NameAfter.text = "";
            MenuSkill3DescriptionAfter.text = "";
        }


        Multishot skill4 = Player.Instance.Multishot;

        MenuSkill4Image.sprite = skill4.Image;
        MenuSkill4NameBefore.text = skill4.Name + " " + skill4.Lvl;
        MenuSkill4DescriptionBefore.text = "Cooldown: " + skill4.Cooldown + "\n ManaCost: " + skill4.ManaCost + "\n Arrows: " + skill4.Arrows;

        if (skill4.Lvl < 5)
        {
            MenuSkill4Cost.text = "Uprgrade cost " + skill4.UpgradeCost();
            MenuSkill4NameAfter.text = skill4.Name + " " + (skill4.Lvl + 1);
            MenuSkill4DescriptionAfter.text = "Cooldown: " + (skill4.Cooldown - ((skill4.Lvl + 1) % 2)) + "\n ManaCost: " + (skill4.ManaCost - 1) + "\n Arrows: " + (skill4.Arrows + ((skill4.Lvl + 1) % 2 == 1 ? 2 : 0));
        }
        else
        {
            MenuSkill4Cost.text = "Max level";
            MenuSkill4NameAfter.text = "";
            MenuSkill4DescriptionAfter.text = "";
        }


        Decoy skill5 = Player.Instance.Decoy;

        MenuSkill5Image.sprite = skill5.Image;
        MenuSkill5NameBefore.text = skill5.Name + " " + skill5.Lvl;
        MenuSkill5DescriptionBefore.text = "Cooldown: " + skill5.Cooldown + "\n ManaCost: " + skill5.ManaCost;

        if (skill5.Lvl < 5)
        {
            MenuSkill5Cost.text = "Uprgrade cost " + skill5.UpgradeCost();
            MenuSkill5NameAfter.text = skill5.Name + " " + (skill5.Lvl + 1);
            MenuSkill5DescriptionAfter.text = "Cooldown: " + (skill5.Cooldown - ((skill5.Lvl + 1) % 2)) + "\n ManaCost: " + (skill5.ManaCost - 2);
        }
        else
        {
            MenuSkill5Cost.text = "Max level";
            MenuSkill5NameAfter.text = "";
            MenuSkill5DescriptionAfter.text = "";
        }
    }

    private void ShowMageSkills()
    {
        Thunderbolt skill1 = Player.Instance.Thunderbolt;

        MenuSkill1Image.sprite = skill1.Image;
        MenuSkill1NameBefore.text = skill1.Name + " " + skill1.Lvl;
        MenuSkill1DescriptionBefore.text = "Cooldown: " + skill1.Cooldown + "\n ManaCost: " + skill1.ManaCost + "\n Damage: " + (Player.Instance.damage + skill1.Damage);

        if (skill1.Lvl < 5)
        {
            MenuSkill1Cost.text = "Uprgrade cost " + skill1.UpgradeCost();
            MenuSkill1NameAfter.text = skill1.Name + " " + (skill1.Lvl + 1);
            MenuSkill1DescriptionAfter.text = "Cooldown: " + (skill1.Cooldown - ((skill1.Lvl + 1) % 2)) + "\n ManaCost: " + (skill1.ManaCost - 2) + "\n Damage: " + (skill1.Damage + 10 + Player.Instance.damage);
        }
        else
        {
            MenuSkill1Cost.text = "Max level";
            MenuSkill1NameAfter.text = "";
            MenuSkill1DescriptionAfter.text = "";
        }


        PoisonMissile skill2 = Player.Instance.PoisonMissile;

        MenuSkill2Image.sprite = skill2.Image;
        MenuSkill2NameBefore.text = skill2.Name + " " + skill2.Lvl;
        MenuSkill2DescriptionBefore.text = "Cooldown: " + skill2.Cooldown + "\n ManaCost: " + skill2.ManaCost + "\n Damage: " + Player.Instance.damage + "\n Venom damage: " + skill2.VenomDamage + "\n Venom time: " + skill2.VenomTime;

        if (skill2.Lvl < 5)
        {
            MenuSkill2Cost.text = "Uprgrade cost " + skill2.UpgradeCost();
            MenuSkill2NameAfter.text = skill2.Name + " " + (skill2.Lvl + 1);
            MenuSkill2DescriptionAfter.text = "Cooldown: " + (skill2.Cooldown - ((skill2.Lvl + 1) % 2)) + "\n ManaCost: " + (skill2.ManaCost - 2) + "\n Damage: " + Player.Instance.damage + "\n  Venom damage: " + (skill2.VenomDamage + 15) + "\n Venom time: " + (skill2.VenomTime + ((skill2.Lvl + 1) % 2));
        }
        else
        {
            MenuSkill2Cost.text = "Max level";
            MenuSkill2NameAfter.text = "";
            MenuSkill2DescriptionAfter.text = "";
        }


        Glyph skill3 = Player.Instance.Glyph;

        MenuSkill3Image.sprite = skill3.Image;
        MenuSkill3NameBefore.text = skill3.Name + " " + skill3.Lvl;
        MenuSkill3DescriptionBefore.text = "Cooldown: " + skill3.Cooldown + "\n ManaCost: " + skill3.ManaCost + "\n Armor: " + skill3.Armor;

        if (skill3.Lvl < 5)
        {
            MenuSkill3Cost.text = "Uprgrade cost " + skill3.UpgradeCost();
            MenuSkill3NameAfter.text = skill3.Name + " " + (skill3.Lvl + 1);
            MenuSkill3DescriptionAfter.text = "Cooldown: " + (skill3.Cooldown - ((skill3.Lvl + 1) % 2)) + "\n ManaCost: " + (skill3.ManaCost - 1) + "\n Armor: " + (skill3.Armor + 5);
        }
        else
        {
            MenuSkill3Cost.text = "Max level";
            MenuSkill3NameAfter.text = "";
            MenuSkill3DescriptionAfter.text = "";
        }


        LifeDrain skill4 = Player.Instance.LifeDrain;

        MenuSkill4Image.sprite = skill4.Image;
        MenuSkill4NameBefore.text = skill4.Name + " " + skill4.Lvl;
        MenuSkill4DescriptionBefore.text = "Cooldown: " + skill4.Cooldown + "\n ManaCost: " + skill4.ManaCost + "\n Damage: " + skill4.Damage + "\n Heal: " + skill4.Heal;

        if (skill4.Lvl < 5)
        {
            MenuSkill4Cost.text = "Uprgrade cost " + skill4.UpgradeCost();
            MenuSkill4NameAfter.text = skill4.Name + " " + (skill4.Lvl + 1);
            MenuSkill4DescriptionAfter.text = "Cooldown: " + (skill4.Cooldown - ((skill4.Lvl + 1) % 2)) + "\n ManaCost: " + (skill4.ManaCost - 1) + "\n Damage: " + (skill4.Damage + 10) + "\n Heal: " + (skill4.Heal + 5);
        }
        else
        {
            MenuSkill4Cost.text = "Max level";
            MenuSkill4NameAfter.text = "";
            MenuSkill4DescriptionAfter.text = "";
        }


        Sacrifice skill5 = Player.Instance.Sacrifice;

        MenuSkill5Image.sprite = skill5.Image;
        MenuSkill5NameBefore.text = skill5.Name + " " + skill5.Lvl;
        MenuSkill5DescriptionBefore.text = "Cooldown: " + skill5.Cooldown + "\n HealthCost: " + skill5.HealthCost;

        if (skill5.Lvl < 5)
        {
            MenuSkill5Cost.text = "Uprgrade cost " + skill5.UpgradeCost();
            MenuSkill5NameAfter.text = skill5.Name + " " + (skill5.Lvl + 1);
            MenuSkill5DescriptionAfter.text = "Cooldown: " + (skill5.Cooldown - ((skill5.Lvl + 1) % 2)) + "\n HealthCost: " + skill5.HealthCost;
        }
        else
        {
            MenuSkill5Cost.text = "Max level";
            MenuSkill5NameAfter.text = "";
            MenuSkill5DescriptionAfter.text = "";
        }
    }

    public void ShowBlacksmith()
    {
        PlayerPanel.SetActive(false);
        MapPanel.SetActive(false);
        InventoryPanel.SetActive(false);
        SkillsPanel.SetActive(false);
        BlacksmithPanel.SetActive(true);
        OptionsPanel.SetActive(false);

        ShowWarriorItems("blacksmith");

        for (int i = 0; i < 16; i++)
            blacksmithSlots[i].Backgorund.color = Color.white;

        if (warriorItems.Count > 0)
        {
            selectedItemSlot = 0;
            blacksmithSlots[selectedItemSlot].Backgorund.color = Color.yellow;
            BlacksmithItemDetails(selectedItemSlot);
        }
    }

    public void ShowOptions()
    {
        PlayerPanel.SetActive(false);
        MapPanel.SetActive(false);
        InventoryPanel.SetActive(false);
        SkillsPanel.SetActive(false);
        BlacksmithPanel.SetActive(false);
        OptionsPanel.SetActive(true);
    }

    public void SellItem()
    {
        if (selectedItemSlot != -1)
        {
            if (currentTab == Tabs.WarriorItems && warriorItems.Count > 0)
            {
                if (Player.Instance.weapon == warriorItems[selectedItemSlot])
                {
                    Player.Instance.damage -= Player.Instance.equipment.GetBonusDamage();
                    Player.Instance.weapon = hand;
                    Player.Instance.equipment.weapon = hand;
                    Player.Instance.damage += Player.Instance.equipment.GetBonusDamage();
                }

                Player.Instance.gold += warriorItems[selectedItemSlot].SellCost();
                warriorItems.RemoveAt(selectedItemSlot);
                ShowWarriorItems("blacksmith");
            }

            if (currentTab == Tabs.RogueItems && rogueItems.Count > 0)
            {
                if (Player.Instance.weapon == rogueItems[selectedItemSlot])
                {
                    Player.Instance.damage -= Player.Instance.equipment.GetBonusDamage();
                    Player.Instance.weapon = hand;
                    Player.Instance.equipment.weapon = hand;
                    Player.Instance.damage += Player.Instance.equipment.GetBonusDamage();
                }

                Player.Instance.gold += rogueItems[selectedItemSlot].SellCost();
                rogueItems.RemoveAt(selectedItemSlot);
                ShowRogueItems("blacksmith");
            }

            if (currentTab == Tabs.MageItems && mageItems.Count > 0)
            {
                if (Player.Instance.weapon == mageItems[selectedItemSlot])
                {
                    Player.Instance.damage -= Player.Instance.equipment.GetBonusDamage();
                    Player.Instance.weapon = hand;
                    Player.Instance.equipment.weapon = hand;
                    Player.Instance.damage += Player.Instance.equipment.GetBonusDamage();
                }

                Player.Instance.gold += mageItems[selectedItemSlot].SellCost();
                mageItems.RemoveAt(selectedItemSlot);
                ShowMageItems("blacksmith");
            }
        }
    }

    public IEnumerator SavePeriodically()
    {
        while (true)
        {
            SaveGame(false);
            yield return new WaitForSeconds(10);
        }
    }

    public void ShowEndingScreen()
    {
        InGameUI.SetActive(true);

        foreach (var gameObject in GameObject.FindGameObjectsWithTag("healthBar"))
            Destroy(gameObject);

        InGameUI.SetActive(false);
        ShowUI.SetActive(false);
        EndingScreen.SetActive(true);

        ReachedDungeonLevel.text = string.Format("You died on level {0} of the fortress.", Player.Instance.EndGameInfo.DungeonLevel);
        ReachedLevel.text = string.Format("Your hero reached level {0}.", Player.Instance.EndGameInfo.LevelGained);
        EnemiesKilled.text = string.Format("You killed {0} enemies.", Player.Instance.EndGameInfo.EnemiesKilled);
        GoldGained.text = string.Format("You looted {0} gold.", Player.Instance.EndGameInfo.GoldGained);
    }

    public void Skill1()
    {
        NormalAttack = false;

        ShieldBash = false;
        BrutalAttack = false;
        WideAttack = false;
        PushingAttack = false;

        Multishot = false;

        Thunderbolt = false;
        PoisonMissile = false;
        LifeDrain = false;

        if (Player.Instance.Class == Player.PlayerClass.Warrior)
        {
            if (Player.Instance.weapon.WeponName.Equals("One handed sword"))
                ShieldBash = true;
            else if (Player.Instance.weapon.WeponName.Equals("Two handed sword"))
                WideAttack = true;
        }
        else if (Player.Instance.Class == Player.PlayerClass.Rogue)
        {
            if (Player.Instance.weapon.WeponName.Equals("Daggers") && InFight && PlayerTurn)
                Player.Instance.PoisonTrap.Use(null);
            else if (Player.Instance.weapon.WeponName.Equals("Bow") && InFight && PlayerTurn)
                Player.Instance.Snare.Use(null);
        }
        else if (Player.Instance.Class == Player.PlayerClass.Mage)
        {
            if (Player.Instance.weapon.WeponName.Equals("Rod"))
                Thunderbolt = true;
            else if (Player.Instance.weapon.WeponName.Equals("Staff") && InFight && PlayerTurn)
                Player.Instance.Glyph.Use(null);
        }       
    }

    public void Skill2()
    {
        NormalAttack = false;

        ShieldBash = false;
        BrutalAttack = false;
        WideAttack = false;
        PushingAttack = false;

        Multishot = false;

        Thunderbolt = false;
        PoisonMissile = false;
        LifeDrain = false;

        if (Player.Instance.Class == Player.PlayerClass.Warrior)
        {
            if (Player.Instance.weapon.WeponName.Equals("One handed sword"))
                BrutalAttack = true;
            else if (Player.Instance.weapon.WeponName.Equals("Two handed sword"))
                PushingAttack = true;
        }
        else if (Player.Instance.Class == Player.PlayerClass.Rogue)
        {
            if (Player.Instance.weapon.WeponName.Equals("Daggers") && InFight && PlayerTurn)
                Player.Instance.Invisibility.Use(null);
            else if (Player.Instance.weapon.WeponName.Equals("Bow"))
                Multishot = true;
        }
        else if (Player.Instance.Class == Player.PlayerClass.Mage)
        {
            if (Player.Instance.weapon.WeponName.Equals("Rod"))
                PoisonMissile = true;
            else if (Player.Instance.weapon.WeponName.Equals("Staff"))
                LifeDrain = true;
        }      
    }

    public void Skill3()
    {
        if (InFight && PlayerTurn)
        {
            if (Player.Instance.Class == Player.PlayerClass.Warrior)
                Player.Instance.BerserkMode.Use(null);
            else if (Player.Instance.Class == Player.PlayerClass.Rogue)
                Player.Instance.Decoy.Use(null);
            else if (Player.Instance.Class == Player.PlayerClass.Mage)
                Player.Instance.Sacrifice.Use(null);
        }
    }


    public void TakeItems()
    {
        Player.Instance.IsUpdateBlocked = true;
        Player.Instance.Invoke("UnblockUpdate", 0.5f);

        Chest.SetActive(false);

        if(ShowUI.activeSelf)
            ShowUI.SetActive(true);
        else
            InGameUI.SetActive(true);

        bool contains = false;

        if (randomedWeapon != null)
        {
            if (randomedWeapon.WeponName.Equals("Two handed sword") || randomedWeapon.WeponName.Equals("One handed sword"))
                if (warriorItems.Count < 16) warriorItems.Add(randomedWeapon);

            if (randomedWeapon.WeponName.Equals("Bow") || randomedWeapon.WeponName.Equals("Daggers"))
                if (rogueItems.Count < 16) rogueItems.Add(randomedWeapon);

            if (randomedWeapon.WeponName.Equals("Rod") || randomedWeapon.WeponName.Equals("Staff"))
                if (mageItems.Count < 16) mageItems.Add(randomedWeapon);
        }

        if (gold != null)
            Player.Instance.gold += gold.amount;

        if (randomedItem != null)
        {
            foreach (var item in Items)
            {
                if (item.itemName == randomedItem.itemName)
                {
                    if(item.amount == 0)
                    {
                        if (item.itemName.Equals("Heal potion"))
                            ChangeAlphaOfImage(Item2Image, 1);

                        if (item.itemName.Equals("Mana potion"))
                            ChangeAlphaOfImage(Item1Image, 1);
                    }     

                    item.amount++;
                    contains = true;
                    break;
                }
            }

            if (!contains)
            {
                Items.Add(randomedItem);
            }
        }

        foreach (Slot slot in chestSlots)
            slot.ClearSlot();

        randomedWeapon = null;
        gold = null;
        randomedItem = null;
        Player.Instance.ChestMenuOn = false;
    }

    public void ToggleMenu()
    {
        MenuOn = !MenuOn;

        InGameUI.SetActive(!InGameUI.activeSelf);
        InGameMenu.SetActive(!InGameMenu.activeSelf);

        if (MenuOn)
            ShowPlayer();
        else
        {
            Player.Instance.IsUpdateBlocked = true;
            Player.Instance.Invoke("UnblockUpdate", 1);
        }
    }

    public void ToggleConfirmPanel()
    {
        ConfirmPanel.SetActive(!ConfirmPanel.activeSelf);
    }

    public void ToggleHUD()
    {
        Player.Instance.IsUpdateBlocked = true;
        Player.Instance.Invoke("UnblockUpdate", 0.5f);

        ShowUI.SetActive(!ShowUI.activeSelf);
        InGameUI.SetActive(!InGameUI.activeSelf);
    }


    private void Update()
    {
        if (Input.GetKeyDown("i"))
            ToggleMenu();

        if (Input.GetKeyDown("space"))
            if (PlayerTurn && InFight)
                ChangeTurnDelayed();
     
        if (MenuOn && Input.GetKeyDown(KeyCode.Escape))
            ToggleMenu();

    }

    public void UpgradeItem()
    {
        if (selectedItemSlot != -1)
        {
            if (currentTab == Tabs.WarriorItems)
                warriorItems[selectedItemSlot].UpgradeWeapon();
            else if (currentTab == Tabs.RogueItems)
                rogueItems[selectedItemSlot].UpgradeWeapon();
            else if (currentTab == Tabs.MageItems)
                mageItems[selectedItemSlot].UpgradeWeapon();

            BlacksmithItemDetails(selectedItemSlot);
        }
    }
}