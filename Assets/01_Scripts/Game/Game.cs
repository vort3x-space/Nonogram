using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Text;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class Game : MonoBehaviour
{
    [Serializable]
    public enum TypeCheck
    {
        None,
        Question,
        Yes
    }

    public struct Point
    {
        public int x;
        public int y;

        public Point(int _x, int _y)
        {
            x = _x;
            y = _y;
        }
    }

    [Serializable]
    public class PosPointXY
    {
        public int x;
        public int y;
        public bool IsPaint;
        public TypeCheck selectTypeCheck;
        public bool HasOK;
    }

    [Serializable]
    public class RowCol
    {
        public int num;
        public bool IsOn;
        public TextMesh text_num;
        public GameObject ImgOK;
        public List<Point> points;
    }



    public static Game instance = null;

    public Button B_Back;                   // Button exit in the level selection menu
    public Button B_Clean;                  // Button clearing the playing field
    public GameObject PanelOK;              // UI if game WIN
    public GameObject PanelLoading;         // UI loading
    public GameObject PanelQuestClear;      // UI question about clear game field
    public GameObject PanelRate;            // UI rate this game (Google, iOS or Amazon)
    public GameObject PanelHelp;            // UI menu help
    [Space(20)]
    public GameObject CanvasShareObj;       // UI expectations shared image
    public Image ImageView;                 // On "PanelOK". Shows unraveled picture
    public Button B_Back_Menu;              // On "PanelOK". Exit in the level selection menu
    public Button B_Next_LVL;               // On "PanelOK". Go next level
    public Button B_Quest_No;               // On "PanelQuestClear". Question answer button "No"
    public Button B_Quest_Yes;              // On "PanelQuestClear". Question answer button "Yes"
    public Button B_Rate_No;                // On "PanelRate". Do not rate the application.
    public Button B_Rate_Yes;               // On "PanelRate". Rate app
    public Button B_Show_Help;              // Button show UI help
    public Button B_Back_Help;              // On "PanelHelp". Button close UI help
    [Space(20)]
    public Tilemap tilemap;                 // Tilemap main playing field
    public Tile tile_none;                  // In Tilemap "tilemap". Tile nothing is selected (void)
    public Tile tile_question;              // In Tilemap "tilemap". Tile we assume that there is something here (question mark)
    public Tile tile_yes;                   // In Tilemap "tilemap". Tile note that it should be drawn here (black square)
    [Space(20)]
    public Transform ColsObj;               // This adds objects with numbers that are in columns.
    public Transform RowsObj;               // This adds objects with numbers that are in rows.
    public GameObject fpref_back_num;       // Prefab "TileText" in which numbers are put
    public RectTransform FonTransform;      // Canvas whith background
    [Space(10)]
    public Tilemap TilemapBack;             // Tilemap draws a background grid in the main playing field
    public Tilemap TilemapBack_Rows;        // Tilemap draws a background grid in a field with numbers for rows
    public Tilemap TilemapBack_Cols;        // Tilemap draws a background grid in a field with numbers for columns
    [Space(10)]
    public Tile tile_back_TL;               // In Tilemap "TilemapBack", "TilemapBack_Rows" and "TilemapBack_Cols". grid
    public Tile tile_back_TC;               // In Tilemap "TilemapBack", "TilemapBack_Rows" and "TilemapBack_Cols". grid
    public Tile tile_back_TR;               // In Tilemap "TilemapBack", "TilemapBack_Rows" and "TilemapBack_Cols". grid
    public Tile tile_back_CL;               // In Tilemap "TilemapBack", "TilemapBack_Rows" and "TilemapBack_Cols". grid
    public Tile tile_back_CC;               // In Tilemap "TilemapBack", "TilemapBack_Rows" and "TilemapBack_Cols". grid
    public Tile tile_back_CR;               // In Tilemap "TilemapBack", "TilemapBack_Rows" and "TilemapBack_Cols". grid
    public Tile tile_back_BL;               // In Tilemap "TilemapBack", "TilemapBack_Rows" and "TilemapBack_Cols". grid
    public Tile tile_back_BC;               // In Tilemap "TilemapBack", "TilemapBack_Rows" and "TilemapBack_Cols". grid
    public Tile tile_back_BR;               // In Tilemap "TilemapBack", "TilemapBack_Rows" and "TilemapBack_Cols". grid
    [Space(10)]
    public Tile tile_back_ONE_LR_T;         // In Tilemap "TilemapBack", "TilemapBack_Rows" and "TilemapBack_Cols". grid
    public Tile tile_back_ONE_LR_C;         // In Tilemap "TilemapBack", "TilemapBack_Rows" and "TilemapBack_Cols". grid
    public Tile tile_back_ONE_LR_B;         // In Tilemap "TilemapBack", "TilemapBack_Rows" and "TilemapBack_Cols". grid
    public Tile tile_back_ONE_TB_L;         // In Tilemap "TilemapBack", "TilemapBack_Rows" and "TilemapBack_Cols". grid
    public Tile tile_back_ONE_TB_C;         // In Tilemap "TilemapBack", "TilemapBack_Rows" and "TilemapBack_Cols". grid
    public Tile tile_back_ONE_TB_R;         // In Tilemap "TilemapBack", "TilemapBack_Rows" and "TilemapBack_Cols". grid
    public Tile tile_back_ONE_CC;           // In Tilemap "TilemapBack", "TilemapBack_Rows" and "TilemapBack_Cols". grid



    [HideInInspector]
    public bool HasPause = false;           // If "true" pause, or "false" play


    private GameSett.ItemsStarted itSt;     // information about the selected level



    private bool IsMove = false;                                // "true" if you made a motion with the taped
    private Vector3 startPos;                                   // position at the beginning of the tapa
    private bool isOn = false;                                  // "true" if you quickly tapped without moving
    private Vector3Int id_click = new Vector3Int(-1, -1, -1);   // cell field for which tap

    private bool IsMoveClick = false;                           // "true" if you squeezed for a certain time and began to move (horizontally or vertically) to sketches
    private float d_Time_move = 0;                              // time counter
    private float d_Time_move_max = 0.75f;                      // time you need to hold tap
    private TypeCheck newTileMove = TypeCheck.None;             // type of sketches when moving
    private Vector2 move_napr = new Vector2(0, 0);              // sketching direction




    List<List<PosPointXY>> field = new List<List<PosPointXY>>();// main field
    List<List<RowCol>> rows = new List<List<RowCol>>();         // numbers in rows
    List<List<RowCol>> cols = new List<List<RowCol>>();         // numbers in columns

    int max_length_row = 0;                                     // max width game field in this level
    int max_length_col = 0;                                     // max height game field in this level



    void Awake()
    {
        IsMove = true;


        try
        {
            instance = this;
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message + " Stack: " + e.StackTrace);
        }

        itSt = GameSett.getInstance().GetSelItem();


        field = GetField(itSt.sp.texture, itSt.sp_mainMenu.texture);
        ResetField();






        B_Back.onClick.RemoveAllListeners();
        B_Back.onClick.AddListener(() =>
        {
            PanelLoading.SetActive(true);

            GameSett.getInstance().UpdMainSp(itSt.lvl);

            GameSett.getInstance().LoadingLevel("Menu", (float a) => { });
        });

        B_Clean.onClick.RemoveAllListeners();
        B_Clean.onClick.AddListener(() =>
        {
            PanelQuestClear.SetActive(true);
            HasPause = true;
        });



        B_Quest_No.onClick.RemoveAllListeners();
        B_Quest_No.onClick.AddListener(() =>
        {
            PanelQuestClear.SetActive(false);
            HasPause = false;
        });

        B_Quest_Yes.onClick.RemoveAllListeners();
        B_Quest_Yes.onClick.AddListener(() =>
        {
            GameSett.getInstance().RestartLevel(itSt.lvl);

            PanelQuestClear.SetActive(false);
            PanelLoading.SetActive(true);

            GameSett.getInstance().SetLevel(itSt.lvl);
            GameSett.getInstance().LoadingLevel("Game", (float a) => { });
        });


        B_Rate_No.onClick.RemoveAllListeners();
        B_Rate_No.onClick.AddListener(() =>
        {
            PanelRate.SetActive(false);
        });

        B_Rate_Yes.onClick.RemoveAllListeners();
        B_Rate_Yes.onClick.AddListener(() =>
        {
            PanelRate.SetActive(false);

            GameSett.getInstance().EndLvlRestart();

        });


        B_Show_Help.onClick.RemoveAllListeners();
        B_Show_Help.onClick.AddListener(() =>
        {
            ShowHelp();
        });

        B_Back_Help.onClick.RemoveAllListeners();
        B_Back_Help.onClick.AddListener(() =>
        {
            PanelHelp.SetActive(false);
            HasPause = false;
        });




        B_Back_Menu.onClick.RemoveAllListeners();
        B_Back_Menu.onClick.AddListener(() =>
        {
            if (ImageView.sprite)
                Destroy(ImageView.sprite);

            PanelLoading.SetActive(true);
            GameSett.getInstance().UpdMainSp(itSt.lvl);
            GameSett.getInstance().LoadingLevel("Menu", (float a) => { });
        });

        B_Next_LVL.onClick.RemoveAllListeners();
        B_Next_LVL.onClick.AddListener(() =>
        {
            if (ImageView.sprite)
                Destroy(ImageView.sprite);

            int I = itSt.lvl + 1;

            PanelLoading.SetActive(true);
            GameSett.getInstance().UpdMainSp(itSt.lvl);

            if (I >= GameSett.getInstance().ItemsSp.Length)
            {
                GameSett.getInstance().LoadingLevel("Menu", (float a) => { });
            }
            else
            {
                GameSett.getInstance().SetLevel(I);
                GameSett.getInstance().LoadingLevel("Game", (float a) => { });
            }
        });

        CanvasShareObj.SetActive(false);


        PanelOK.SetActive(false);
        PanelLoading.SetActive(false);
        HasPause = false;


        if (!GameSett.getInstance().HasFirstStart())
        {
            GameSett.getInstance().SetFirstStart();
            ShowHelp();
        }
    }

    void Start()
    {
        // determine the boundaries of the camera movement
        PinchZoom.Instance().SetGranici(-itSt.sp.texture.width, -(itSt.sp.texture.height + max_length_col), max_length_row, 0);
        PinchZoom.Instance().ResetPosition();


        // stretch canvas background
        FonTransform.localPosition = new Vector3((itSt.sp.texture.width - max_length_row) / 2.0f, (itSt.sp.texture.height + max_length_col) / 2.0f, 0);
        FonTransform.sizeDelta = new Vector2(itSt.sp.texture.width + max_length_row + 10, itSt.sp.texture.height + max_length_col + 10);
    }

    //create a playing field
    List<List<PosPointXY>> GetField(Texture2D tex, Texture2D tex_paint)
    {
        List<List<PosPointXY>> co = new List<List<PosPointXY>>(); // tmp game field
        Color[] cc = tex.GetPixels();
        Color[] cc_paint = tex_paint.GetPixels();
        int width = tex.width;
        int height = tex.height;

        Color black = new Color(0, 0, 0, 1);


        #region  create a playing field and its grid in the background, and determine what type each cell
        for (int x = 0; x < width; x++)
        {
            List<PosPointXY> row_point = new List<PosPointXY>();
            for (int y = 0; y < height; y++)
            {
                int id = y * width + x;

                PosPointXY point = new PosPointXY()
                    {
                        HasOK = false,
                        IsPaint = false,
                        selectTypeCheck = (cc_paint[id] == black) ? TypeCheck.Yes : TypeCheck.None,
                        x = x,
                        y = y
                    };

                if (cc[id] == black)
                {
                    point.IsPaint = true;
                }

                row_point.Add(point);

                #region Background
                Tile tile = tile_back_CC;
                bool b_x = ((x + 1) % 5 == 0 || (x + 1) == width);
                bool b_x0 = (x % 5 == 0 || x == 0);
                bool b_y = ((height - (y + 1)) % 5 == 0 || (y + 1) == height);
                bool b_y0 = ((height - y) % 5 == 0 || y == 0);

                if (b_x && !b_x0 && b_y && !b_y0)
                    tile = tile_back_TR;
                else if (b_x && !b_x0 && !b_y && !b_y0)
                    tile = tile_back_CR;
                else if (b_x && !b_x0 && !b_y && b_y0)
                    tile = tile_back_BR;

                else if (!b_x && b_x0 && b_y && !b_y0)
                    tile = tile_back_TL;
                else if (!b_x && b_x0 && !b_y && !b_y0)
                    tile = tile_back_CL;
                else if (!b_x && b_x0 && !b_y && b_y0)
                    tile = tile_back_BL;

                else if (!b_x && !b_x0 && b_y && !b_y0)
                    tile = tile_back_TC;
                else if (!b_x && !b_x0 && !b_y && !b_y0)
                    tile = tile_back_CC;
                else if (!b_x && !b_x0 && !b_y && b_y0)
                    tile = tile_back_BC;


                else if (b_x && b_x0 && b_y && !b_y0)
                    tile = tile_back_ONE_LR_T;
                else if (b_x && b_x0 && !b_y && !b_y0)
                    tile = tile_back_ONE_LR_C;
                else if (b_x && b_x0 && !b_y && b_y0)
                    tile = tile_back_ONE_LR_B;

                else if (b_x && !b_x0 && b_y && b_y0)
                    tile = tile_back_ONE_TB_R;
                else if (!b_x && !b_x0 && b_y && b_y0)
                    tile = tile_back_ONE_TB_C;
                else if (!b_x && b_x0 && b_y && b_y0)
                    tile = tile_back_ONE_TB_L;

                else if (b_x && b_x0 && b_y && b_y0)
                    tile = tile_back_ONE_CC;

                TilemapBack.SetTile(new Vector3Int(x, y, 0), tile);
                #endregion
            }
            co.Add(row_point);
        }
        #endregion


        max_length_row = 0;
        max_length_col = 0;

        #region numbers in columns
        List<Point> points = new List<Point>();

        for (int x = 0; x < width; x++)
        {
            List<RowCol> col_tmp = new List<RowCol>();
            int count = 0;
            for (int y = 0; y < height; y++)
            {
                if (co[x][y].IsPaint)
                {
                    count++;
                    points.Add(new Point(x, y));
                }

                if ((!co[x][y].IsPaint || (y + 1) == height) && count != 0)
                {
                    col_tmp.Add(new RowCol()
                    {
                        IsOn = false,
                        num = count,
                        points = points
                    });
                    count = 0;
                    points = new List<Point>();
                }
            }
            cols.Add(col_tmp);

            if (col_tmp.Count > max_length_col)
                max_length_col = col_tmp.Count;
        }
        #endregion


        #region numbers in rows
        points = new List<Point>();

        for (int y = 0; y < height; y++)
        {
            List<RowCol> row_tmp = new List<RowCol>();
            int count = 0;
            for (int x = 0; x < width; x++)
            {
                if (co[x][y].IsPaint)
                {
                    count++;
                    points.Add(new Point(x, y));
                }

                if ((!co[x][y].IsPaint || (x + 1) == width) && count != 0)
                {
                    row_tmp.Add(new RowCol()
                    {
                        IsOn = false,
                        num = count,
                        points = points
                    });
                    count = 0;
                    points = new List<Point>();
                }
            }
            rows.Add(row_tmp);

            if (row_tmp.Count > max_length_row)
                max_length_row = row_tmp.Count;
        }
        #endregion


        #region Background and numbs for cols and rows
        TilemapBack_Cols.transform.localPosition = new Vector3(0, height, 0);
        TilemapBack_Rows.transform.localPosition = new Vector3(-max_length_row, 0, 0);

        ColsObj.transform.localPosition = new Vector3(0, height, 0);
        RowsObj.transform.localPosition = new Vector3(-max_length_row, 0, 0);


        for (int x = 0; x < cols.Count; x++)
        {
            for (int y = 0; y < max_length_col; y++)
            {
                Tile tile_cr = tile_back_CC;
                bool b_x_cr = ((x + 1) % 5 == 0 || (x + 1) == cols.Count);
                bool b_x0_cr = (x % 5 == 0 || x == 0);
                bool b_y_cr = ((y + 1) % 5 == 0 || (y + 1) == max_length_col);
                bool b_y0_cr = (y % 5 == 0 || y == 0);

                if (b_x_cr && !b_x0_cr && b_y_cr && !b_y0_cr)
                    tile_cr = tile_back_TR;
                else if (b_x_cr && !b_x0_cr && !b_y_cr && !b_y0_cr)
                    tile_cr = tile_back_CR;
                else if (b_x_cr && !b_x0_cr && !b_y_cr && b_y0_cr)
                    tile_cr = tile_back_BR;

                else if (!b_x_cr && b_x0_cr && b_y_cr && !b_y0_cr)
                    tile_cr = tile_back_TL;
                else if (!b_x_cr && b_x0_cr && !b_y_cr && !b_y0_cr)
                    tile_cr = tile_back_CL;
                else if (!b_x_cr && b_x0_cr && !b_y_cr && b_y0_cr)
                    tile_cr = tile_back_BL;

                else if (!b_x_cr && !b_x0_cr && b_y_cr && !b_y0_cr)
                    tile_cr = tile_back_TC;
                else if (!b_x_cr && !b_x0_cr && !b_y_cr && !b_y0_cr)
                    tile_cr = tile_back_CC;
                else if (!b_x_cr && !b_x0_cr && !b_y_cr && b_y0_cr)
                    tile_cr = tile_back_BC;


                else if (b_x_cr && b_x0_cr && b_y_cr && !b_y0_cr)
                    tile_cr = tile_back_ONE_LR_T;
                else if (b_x_cr && b_x0_cr && !b_y_cr && !b_y0_cr)
                    tile_cr = tile_back_ONE_LR_C;
                else if (b_x_cr && b_x0_cr && !b_y_cr && b_y0_cr)
                    tile_cr = tile_back_ONE_LR_B;

                else if (b_x_cr && !b_x0_cr && b_y_cr && b_y0_cr)
                    tile_cr = tile_back_ONE_TB_R;
                else if (!b_x_cr && !b_x0_cr && b_y_cr && b_y0_cr)
                    tile_cr = tile_back_ONE_TB_C;
                else if (!b_x_cr && b_x0_cr && b_y_cr && b_y0_cr)
                    tile_cr = tile_back_ONE_TB_L;

                else if (b_x_cr && b_x0_cr && b_y_cr && b_y0_cr)
                    tile_cr = tile_back_ONE_CC;

                TilemapBack_Cols.SetTile(new Vector3Int(x, y, 0), tile_cr);

                if (y < cols[x].Count)
                {
                    GameObject go = null;// new GameObject();
                    cols[x][y].text_num = AddNumTile(ColsObj, TilemapBack_Cols, x, y, cols[x][y].num, ref go);
                    cols[x][y].ImgOK = go;
                }
            }
        }

        for (int y = 0; y < rows.Count; y++)
        {
            for (int x = 0; x < max_length_row; x++)
            {
                Tile tile_cr = tile_back_CC;
                bool b_x_cr = ((max_length_row - (x + 1)) % 5 == 0 || (x + 1) == max_length_row);
                bool b_x0_cr = ((max_length_row - x) % 5 == 0 || x == 0);
                bool b_y_cr = ((rows.Count - (y + 1)) % 5 == 0 || (y + 1) == rows.Count);
                bool b_y0_cr = ((rows.Count - y) % 5 == 0 || y == 0);

                if (b_x_cr && !b_x0_cr && b_y_cr && !b_y0_cr)
                    tile_cr = tile_back_TR;
                else if (b_x_cr && !b_x0_cr && !b_y_cr && !b_y0_cr)
                    tile_cr = tile_back_CR;
                else if (b_x_cr && !b_x0_cr && !b_y_cr && b_y0_cr)
                    tile_cr = tile_back_BR;

                else if (!b_x_cr && b_x0_cr && b_y_cr && !b_y0_cr)
                    tile_cr = tile_back_TL;
                else if (!b_x_cr && b_x0_cr && !b_y_cr && !b_y0_cr)
                    tile_cr = tile_back_CL;
                else if (!b_x_cr && b_x0_cr && !b_y_cr && b_y0_cr)
                    tile_cr = tile_back_BL;

                else if (!b_x_cr && !b_x0_cr && b_y_cr && !b_y0_cr)
                    tile_cr = tile_back_TC;
                else if (!b_x_cr && !b_x0_cr && !b_y_cr && !b_y0_cr)
                    tile_cr = tile_back_CC;
                else if (!b_x_cr && !b_x0_cr && !b_y_cr && b_y0_cr)
                    tile_cr = tile_back_BC;


                else if (b_x_cr && b_x0_cr && b_y_cr && !b_y0_cr)
                    tile_cr = tile_back_ONE_LR_T;
                else if (b_x_cr && b_x0_cr && !b_y_cr && !b_y0_cr)
                    tile_cr = tile_back_ONE_LR_C;
                else if (b_x_cr && b_x0_cr && !b_y_cr && b_y0_cr)
                    tile_cr = tile_back_ONE_LR_B;

                else if (b_x_cr && !b_x0_cr && b_y_cr && b_y0_cr)
                    tile_cr = tile_back_ONE_TB_R;
                else if (!b_x_cr && !b_x0_cr && b_y_cr && b_y0_cr)
                    tile_cr = tile_back_ONE_TB_C;
                else if (!b_x_cr && b_x0_cr && b_y_cr && b_y0_cr)
                    tile_cr = tile_back_ONE_TB_L;

                else if (b_x_cr && b_x0_cr && b_y_cr && b_y0_cr)
                    tile_cr = tile_back_ONE_CC;

                TilemapBack_Rows.SetTile(new Vector3Int(x, y, 0), tile_cr);

                if (x < rows[y].Count)
                {
                    GameObject go = null;// new GameObject();
                    rows[y][x].text_num = AddNumTile(RowsObj, TilemapBack_Rows, (max_length_row - rows[y].Count) + x, y, rows[y][x].num, ref go);
                    rows[y][x].ImgOK = go;
                }
            }
        }
        #endregion

        return co;
    }

    // create TextMesh for numbers
    TextMesh AddNumTile(Transform tr, Tilemap tm, int x, int y, int num, ref GameObject go_sp)
    {
        GameObject o = GameObject.Instantiate(fpref_back_num);
        o.name += "_" + x.ToString() + "_" + y.ToString();
        o.transform.SetParent(tr, false);
        o.transform.localPosition = tm.CellToLocal(new Vector3Int(x, y, 0));

        TextMesh tt = o.GetComponentInChildren<TextMesh>();
        go_sp = o.GetComponentInChildren<SpriteRenderer>().gameObject;

        tt.text = num.ToString();

        return tt;
    }

    // sets tiles at the beginning of the game
    void ResetField()
    {
        for (int x = 0; x < field.Count; x++)
        {
            for (int y = 0; y < field[x].Count; y++)
            {
                SetTile(new Vector3Int(x, y, 0), field[x][y].selectTypeCheck);
            }
        }
    }

    void ShowHelp()
    {
        PanelHelp.SetActive(true);
        HasPause = true;
    }


    // returns cell numbers ('x' and 'y') at specified position
    Vector3Int GetIdCell(Vector3 pos)
    {
        Vector3Int cellPosition = tilemap.WorldToCell(pos);

        int width = itSt.sp.texture.width;
        int height = itSt.sp.texture.height;

        if (cellPosition.x >= width || cellPosition.y >= height || cellPosition.x < 0 || cellPosition.y < 0)
        {
            cellPosition = new Vector3Int(-1, -1, -1);
        }

        return cellPosition;
    }

    // change and set the type of choice in the cell
    void ClickTile(Vector3Int pos)
    {
        if (pos.x != -1)
        {
            TypeCheck tc_new = GetNewTile(pos);

            SetTile(pos, tc_new);
        }
    }

    // changes the type of the selected cell and returns a new (by cycle)
    TypeCheck GetNewTile(Vector3Int pos)
    {
        TypeCheck tc_new = TypeCheck.None;

        if (pos.x != -1)
        {
            int i = pos.x;
            int j = pos.y;

            TypeCheck tc = field[i][j].selectTypeCheck;

            if (tc == TypeCheck.None)
                tc_new = TypeCheck.Yes;
            if (tc == TypeCheck.Yes)
                tc_new = TypeCheck.Question;
        }

        return tc_new;
    }

    // sets a new cell type and changes to a tile
    void SetTile(Vector3Int pos, TypeCheck tc_new)
    {
        if (pos.x != -1)
        {
            int i = pos.x;
            int j = pos.y;

            Tile tile = tile_none;

            if (tc_new == TypeCheck.Yes)
                tile = tile_yes;
            if (tc_new == TypeCheck.Question)
                tile = tile_question;

            tilemap.SetTile(pos, tile);

            field[i][j].selectTypeCheck = tc_new;
            field[i][j].HasOK = ((tc_new == TypeCheck.Yes && field[i][j].IsPaint) || (tc_new != TypeCheck.Yes && !field[i][j].IsPaint)) ? true : false;

            itSt.sp_mainMenu.texture.SetPixel(pos.x, pos.y, (field[i][j].selectTypeCheck == TypeCheck.Yes) ? new Color(0, 0, 0, 1) : new Color(1, 1, 1, 1));
            itSt.sp_mainMenu.texture.Apply();

            UpdNum(i, j);

            Save();
        }
    }

    // determine which numbers in the columns and rows are already correctly selected
    void UpdNum(int x, int y)
    {
        int width = field.Count;
        int height = field[x].Count;

        for (int i = 0; i < rows[y].Count; i++)
        {
            rows[y][i].IsOn = true;
        }

        for (int j = 0; j < cols[x].Count; j++)
        {
            cols[x][j].IsOn = true;
        }

        int _i = 0;
        int _j = 0;
        bool b = true;

        if (rows[y].Count > 0)
        {
            for (int i = 0; i < width; i++)
            {
                int min = (_i < rows[y].Count && rows[y][_i].points.Count > 0) ? rows[y][_i].points[0].x : -1;
                int max = (_i < rows[y].Count && rows[y][_i].points.Count > 0) ? rows[y][_i].points[rows[y][_i].points.Count - 1].x : -1;

                if (i >= min && i <= max)
                {
                    if (!field[i][y].HasOK)
                    {
                        rows[y][_i].IsOn = false;
                        i = max;
                        _i++;
                    }
                    else
                    {
                        if (i == max)
                        {
                            _i++;
                        }
                    }
                }
                else
                {
                    if (!field[i][y].HasOK)
                    {
                        b = false;
                    }
                }
            }

            if (!b)
            {
                for (int i = 0; i < rows[y].Count; i++)
                {
                    rows[y][i].IsOn = false;
                }
            }
        }

        b = true;
        if (cols[x].Count > 0)
        {
            for (int j = 0; j < height; j++)
            {
                int min = (_j < cols[x].Count && cols[x][_j].points.Count > 0) ? cols[x][_j].points[0].y : -1;
                int max = (_j < cols[x].Count && cols[x][_j].points.Count > 0) ? cols[x][_j].points[cols[x][_j].points.Count - 1].y : -1;

                if (j >= min && j <= max)
                {
                    if (!field[x][j].HasOK)
                    {
                        cols[x][_j].IsOn = false;
                        j = max;
                        _j++;
                    }
                    else
                    {
                        if (j == max)
                        {
                            _j++;
                        }
                    }
                }
                else
                {
                    if (!field[x][j].HasOK)
                    {
                        b = false;
                    }
                }
            }

            if (!b)
            {
                for (int j = 0; j < cols[x].Count; j++)
                {
                    cols[x][j].IsOn = false;
                }
            }
        }


        for (int i = 0; i < rows[y].Count; i++)
        {
            if (rows[y][i].IsOn)
                rows[y][i].ImgOK.SetActive(true);
            else
                rows[y][i].ImgOK.SetActive(false);
        }

        for (int j = 0; j < cols[x].Count; j++)
        {
            if (cols[x][j].IsOn)
                cols[x][j].ImgOK.SetActive(true);
            else
                cols[x][j].ImgOK.SetActive(false);
        }
    }

    // tap action
    void UpdMous()
    {
        if (!HasPause)
        {
            d_Time_move += Time.deltaTime;

            if (Input.touchCount == 0) // for mouse (in editor or for Windows)
            {
                if (!IsMove && !IsMoveClick && Input.GetMouseButton(0) && d_Time_move >= d_Time_move_max) // if it was clamped a certain time, then you can make a sketch of the movement
                {
                    Vector3 pos = Camera.main.ScreenToWorldPoint(startPos);
                    id_click = GetIdCell(pos);
                    newTileMove = GetNewTile(id_click);

                    if (id_click.x != -1) // fall on the playing field
                    {
                        IsMove = true;
                        IsMoveClick = true;
                        move_napr = new Vector2(0, 0);
                    }
                }
                if (!IsMove && Input.GetMouseButton(0) && Vector3.Distance(Input.mousePosition, startPos) > 1f) // started moving
                {
                    IsMove = true;
                    IsMoveClick = false;
                }
                else if (Input.GetMouseButtonDown(0)) // just clicked
                {
                    d_Time_move = 0;
                    startPos = Input.mousePosition;
                    IsMove = false;
                    IsMoveClick = false;
                }

                if (!IsMove && Input.GetMouseButtonUp(0)) // if you quickly tapped
                {
                    Vector3 pos = Camera.main.ScreenToWorldPoint(startPos);

                    id_click = GetIdCell(pos);

                    if (!MyRay.getInstance().HasUI(startPos)) // nothing overlaps from UI
                    {
                        if (!isOn)
                        {
                            isOn = true;
                        }
                    }

                    IsMove = true;
                }
                if (Input.GetMouseButtonUp(0)) // removed your finger from the screen
                    IsMoveClick = false;
            }
            else if (Input.touchCount == 1) // for phones or tablets
            {
                if (!IsMove && !IsMoveClick && d_Time_move >= d_Time_move_max) // if it was clamped a certain time, then you can make a sketch of the movement
                {
                    Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y));
                    id_click = GetIdCell(pos);
                    newTileMove = GetNewTile(id_click);

                    if (id_click.x != -1) // fall on the playing field
                    {
                        IsMove = true;
                        IsMoveClick = true;
                        move_napr = new Vector2(0, 0);
                    }
                }
                if (!IsMove && Input.GetTouch(0).phase == TouchPhase.Moved) // started moving
                {
                    IsMove = true;
                }
                else if (Input.GetTouch(0).phase == TouchPhase.Began) // just clicked
                {
                    d_Time_move = 0;
                    startPos = Input.GetTouch(0).position;
                    IsMove = false;
                    IsMoveClick = false;
                }

                if (!IsMove && Input.GetTouch(0).phase == TouchPhase.Ended) // if you quickly tapped
                {
                    Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y));

                    id_click = GetIdCell(pos);

                    if (!MyRay.getInstance().HasUI(Input.GetTouch(0).position)) // nothing overlaps from UI
                    {
                        if (!isOn)
                        {
                            isOn = true;
                        }
                    }

                    IsMove = true;
                }
                if (Input.GetTouch(0).phase == TouchPhase.Ended) // removed your finger from the screen
                    IsMoveClick = false;
            }


            if (isOn) // single tap without movement
            {
                ClickTile(id_click);

                id_click = new Vector3Int(-1, -1, -1);
                isOn = false;
            }


            if (IsMoveClick) // sketching with taped
            {
                if (Input.touchCount == 0) // for mouse (in editor or for Windows)
                {
                    if (move_napr.x == 0 && move_napr.y == 0)
                    {
                        Vector2 v2_del = startPos - Input.mousePosition;
                        if (Math.Abs(v2_del.x) > Math.Abs(v2_del.y))
                            move_napr = new Vector2(1, 0);
                        else if (Math.Abs(v2_del.x) < Math.Abs(v2_del.y))
                            move_napr = new Vector2(0, 1);
                    }
                    Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3((move_napr.x == 1) ? Input.mousePosition.x : startPos.x
                        , (move_napr.y == 1) ? Input.mousePosition.y : startPos.y));
                    id_click = GetIdCell(pos);
                }
                else if (Input.touchCount == 1) // for phones or tablets
                {
                    if (move_napr.x == 0 && move_napr.y == 0)
                    {
                        Vector2 v2_del = new Vector2(startPos.x, startPos.y) - Input.GetTouch(0).position;
                        if (Math.Abs(v2_del.x) > Math.Abs(v2_del.y))
                            move_napr = new Vector2(1, 0);
                        else if (Math.Abs(v2_del.x) < Math.Abs(v2_del.y))
                            move_napr = new Vector2(0, 1);
                    }
                    Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3((move_napr.x == 1) ? Input.GetTouch(0).position.x : startPos.x
                        , (move_napr.y == 1) ? Input.GetTouch(0).position.y : startPos.y));
                    id_click = GetIdCell(pos);
                }

                SetTile(id_click, newTileMove);
            }
        }
    }

    // check whether the picture is finished and if the "true" then show the end UI
    void TheEnd()
    {
        bool b_end = true;

        for (int i = 0; i < field.Count; i++)
        {
            for (int j = 0; j < field[i].Count; j++)
            {
                if (!field[i][j].HasOK)
                {
                    b_end = false;
                    break;
                }
            }
            if (!b_end)
                break;
        }

        if (b_end)
        {
            PinchZoom.Instance().ResetPosition();
            HasPause = true;
            PanelOK.SetActive(true);

            GameSett.getInstance().EndLvl();
            if (GameSett.getInstance().GetEndLvl() >= 5)
            {
                PanelRate.SetActive(true);
            }
            
            Texture2D texture_sp_mainMenu = new Texture2D(itSt.sp_mainMenu.texture.width, itSt.sp_mainMenu.texture.height, TextureFormat.RGBA32, false);
            texture_sp_mainMenu.filterMode = FilterMode.Point;
            texture_sp_mainMenu.SetPixels(itSt.sp_mainMenu.texture.GetPixels());
            texture_sp_mainMenu.Apply();

            Rect r_sp_mainMenu = new Rect(0, 0, texture_sp_mainMenu.width, texture_sp_mainMenu.height);
            Sprite sprite_sp_mainMenu = Sprite.Create(texture_sp_mainMenu, r_sp_mainMenu, new Vector2(0.5f, 0.5f), itSt.sp_mainMenu.pixelsPerUnit);

            ImageView.sprite = sprite_sp_mainMenu;
        }
    }

    void Update()
    {
        PinchZoom.Instance().SetPause(HasPause || IsMoveClick);

        if (!HasPause)
        {
            UpdMous();
            TheEnd();
        }
    }


    // save changes to the game
    void Save()
    {
        try
        {
            SaveGameXML.SaveLvl(itSt.sp_mainMenu.texture, itSt.lvl);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Save - " + e.ToString());
        }
    }





    public static Game getInstance()
    {
        return instance;
    }
}
