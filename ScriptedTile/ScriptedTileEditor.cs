using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace EllenExplorer.Tools.Tiles {
    #region @CLASSROOM: ScriptableObject e Inspector.
    /**
     * Todo ScriptableObject pode ter seu Inspector modificado para deixá-lo mais agradavel.
     * 
     * ScriptedTile estende de TileBase, que por sua vez, estende de ScriptableObject.
     * Logo TileBase e ScriptedTile possuem Inspectors customizáveis.
     * 
     * Então, todo ScriptableObject do tipo RuleTile terá seu Inspector customizado.
     */
    #endregion
    [CustomEditor(typeof(ScriptedTile), true)]
    [CanEditMultipleObjects]
    internal class ScriptedTileEditor : Editor {
        #region @CLASSROOM: `target`.
        /**
         * `target` vem do Editor.
         * É o ScriptableObject que esta sendo inspecionado no momento.
         * Neste caso o ScriptedTile.
         */
        #endregion
        public ScriptedTile scriptedTile => target as ScriptedTile;

        public ReorderableList reorderableListOfAllRules;

        private static class ReorderableListGUIDefaults {
            public const float ComponentWidth = 48;
            public const float FieldWidth = 90f;
            public const float FieldHeight = 18f;
            public const float FieldPaddingTop = 1f;

            public const float ElementHeight = 48;
            public const float ElementPaddingHeight = 26;
        }

        private List<int> neighborRuleCodes;

        private const string s_XIconString = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPCAYAAAA71pVKAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAYdEVYdFNvZnR3YXJlAHBhaW50Lm5ldCA0LjAuNWWFMmUAAABoSURBVDhPnY3BDcAgDAOZhS14dP1O0x2C/LBEgiNSHvfwyZabmV0jZRUpq2zi6f0DJwdcQOEdwwDLypF0zHLMa9+NQRxkQ+ACOT2STVw/q8eY1346ZlE54sYAhVhSDrjwFymrSFnD2gTZpls2OvFUHAAAAABJRU5ErkJggg==";
        private const string s_Arrow0 = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPCAYAAAA71pVKAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAYdEVYdFNvZnR3YXJlAHBhaW50Lm5ldCA0LjAuNWWFMmUAAACYSURBVDhPzZExDoQwDATzE4oU4QXXcgUFj+YxtETwgpMwXuFcwMFSRMVKKwzZcWzhiMg91jtg34XIntkre5EaT7yjjhI9pOD5Mw5k2X/DdUwFr3cQ7Pu23E/BiwXyWSOxrNqx+ewnsayam5OLBtbOGPUM/r93YZL4/dhpR/amwByGFBz170gNChA6w5bQQMqramBTgJ+Z3A58WuWejPCaHQAAAABJRU5ErkJggg==";
        private const string s_Arrow1 = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPCAYAAAA71pVKAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAYdEVYdFNvZnR3YXJlAHBhaW50Lm5ldCA0LjAuNWWFMmUAAABqSURBVDhPxYzBDYAgEATpxYcd+PVr0fZ2siZrjmMhFz6STIiDs8XMlpEyi5RkO/d66TcgJUB43JfNBqRkSEYDnYjhbKD5GIUkDqRDwoH3+NgTAw+bL/aoOP4DOgH+iwECEt+IlFmkzGHlAYKAWF9R8zUnAAAAAElFTkSuQmCC";
        private const string s_Arrow2 = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPCAYAAAA71pVKAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAYdEVYdFNvZnR3YXJlAHBhaW50Lm5ldCA0LjAuNWWFMmUAAAC0SURBVDhPjVE5EsIwDMxPKFKYF9CagoJH8xhaMskLmEGsjOSRkBzYmU2s9a58TUQUmCH1BWEHweuKP+D8tphrWcAHuIGrjPnPNY8X2+DzEWE+FzrdrkNyg2YGNNfRGlyOaZDJOxBrDhgOowaYW8UW0Vau5ZkFmXbbDr+CzOHKmLinAXMEePyZ9dZkZR+s5QX2O8DY3zZ/sgYcdDqeEVp8516o0QQV1qeMwg6C91toYoLoo+kNt/tpKQEVvFQAAAAASUVORK5CYII=";
        private const string s_Arrow3 = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPCAYAAAA71pVKAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAYdEVYdFNvZnR3YXJlAHBhaW50Lm5ldCA0LjAuNWWFMmUAAAB2SURBVDhPzY1LCoAwEEPnLi48gW5d6p31bH5SMhp0Cq0g+CCLxrzRPqMZ2pRqKG4IqzJc7JepTlbRZXYpWTg4RZE1XAso8VHFKNhQuTjKtZvHUNCEMogO4K3BhvMn9wP4EzoPZ3n0AGTW5fiBVzLAAYTP32C2Ay3agtu9V/9PAAAAAElFTkSuQmCC";
        private const string s_Arrow5 = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPCAYAAAA71pVKAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAYdEVYdFNvZnR3YXJlAHBhaW50Lm5ldCA0LjAuNWWFMmUAAABqSURBVDhPnY3BCYBADASvFx924NevRdvbyoLBmNuDJQMDGjNxAFhK1DyUQ9fvobCdO+j7+sOKj/uSB+xYHZAxl7IR1wNTXJeVcaAVU+614uWfCT9mVUhknMlxDokd15BYsQrJFHeUQ0+MB5ErsPi/6hO1AAAAAElFTkSuQmCC";
        private const string s_Arrow6 = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPCAYAAAA71pVKAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAYdEVYdFNvZnR3YXJlAHBhaW50Lm5ldCA0LjAuNWWFMmUAAACaSURBVDhPxZExEkAwEEVzE4UiTqClUDi0w2hlOIEZsV82xCZmQuPPfFn8t1mirLWf7S5flQOXjd64vCuEKWTKVt+6AayH3tIa7yLg6Qh2FcKFB72jBgJeziA1CMHzeaNHjkfwnAK86f3KUafU2ClHIJSzs/8HHLv09M3SaMCxS7ljw/IYJWzQABOQZ66x4h614ahTCL/WT7BSO51b5Z5hSx88AAAAAElFTkSuQmCC";
        private const string s_Arrow7 = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPCAYAAAA71pVKAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAYdEVYdFNvZnR3YXJlAHBhaW50Lm5ldCA0LjAuNWWFMmUAAABQSURBVDhPYxh8QNle/T8U/4MKEQdAmsz2eICx6W530gygr2aQBmSMphkZYxqErAEXxusKfAYQ7XyyNMIAsgEkaYQBkAFkaYQBsjXSGDAwAAD193z4luKPrAAAAABJRU5ErkJggg==";
        private const string s_Arrow8 = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPCAYAAAA71pVKAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAYdEVYdFNvZnR3YXJlAHBhaW50Lm5ldCA0LjAuNWWFMmUAAACYSURBVDhPxZE9DoAwCIW9iUOHegJXHRw8tIdx1egJTMSHAeMPaHSR5KVQ+KCkCRF91mdz4VDEWVzXTBgg5U1N5wahjHzXS3iFFVRxAygNVaZxJ6VHGIl2D6oUXP0ijlJuTp724FnID1Lq7uw2QM5+thoKth0N+GGyA7IA3+yM77Ag1e2zkey5gCdAg/h8csy+/89v7E+YkgUntOWeVt2SfAAAAABJRU5ErkJggg==";
        private const string s_MirrorX = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPCAYAAAA71pVKAAAABGdBTUEAALGPC/xhBQAAAAlwSFlzAAAOwQAADsEBuJFr7QAAABh0RVh0U29mdHdhcmUAcGFpbnQubmV0IDQuMC41ZYUyZQAAAG1JREFUOE+lj9ENwCAIRB2IFdyRfRiuDSaXAF4MrR9P5eRhHGb2Gxp2oaEjIovTXSrAnPNx6hlgyCZ7o6omOdYOldGIZhAziEmOTSfigLV0RYAB9y9f/7kO8L3WUaQyhCgz0dmCL9CwCw172HgBeyG6oloC8fAAAAAASUVORK5CYII=";
        private const string s_MirrorY = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPCAYAAAA71pVKAAAABGdBTUEAALGPC/xhBQAAAAlwSFlzAAAOwgAADsIBFShKgAAAABh0RVh0U29mdHdhcmUAcGFpbnQubmV0IDQuMC41ZYUyZQAAAG9JREFUOE+djckNACEMAykoLdAjHbPyw1IOJ0L7mAejjFlm9hspyd77Kk+kBAjPOXcakJIh6QaKyOE0EB5dSPJAiUmOiL8PMVGxugsP/0OOib8vsY8yYwy6gRyC8CB5QIWgCMKBLgRSkikEUr5h6wOPWfMoCYILdgAAAABJRU5ErkJggg==";
        private const string s_MirrorXY = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPCAYAAAA71pVKAAAABGdBTUEAALGPC/xhBQAAAAlwSFlzAAAOwgAADsIBFShKgAAAABl0RVh0U29mdHdhcmUAcGFpbnQubmV0IDQuMC4yMfEgaZUAAAHkSURBVDhPrVJLSwJRFJ4cdXwjPlrVJly1kB62cpEguElXKgYKIpaC+EIEEfGxLqI/UES1KaJlEdGmRY9ltCsIWrUJatGm0eZO3xkHIsJdH3zce+ec75z5zr3cf2MMmLdYLA/BYFA2mUyPOPvwnR+GR4PXaDQLLpfrKpVKSb1eT6bV6XTeocAS4sIw7S804BzEZ4IgsGq1ykhcr9dlj8czwPdbxJdBMyX/As/zLiz74Ar2J9lsVulcKpUYut5DnEbsHFwEx8AhtFqtGViD6BOc1ul0B5lMRhGXy2Wm1+ufkBOE/2fsL1FsQpXCiCAcQiAlk0kJRZjf7+9TRxI3Gg0WCoW+IpGISHHERBS5UKUch8n2K5WK3O125VqtpqydTkdZie12W261WjIVo73b7RZVKccZDIZ1q9XaT6fTLB6PD9BFKhQKjITFYpGFw+FBNBpVOgcCARH516pUGZYZXk5R4B3efLBxDM9f1CkWi/WR3ICtGVh6Rd4NPE+p0iEgmkSRLRoMEjYhHpA4kUiIOO8iZRU8AmnadK2/QOOfhnjPZrO95fN5Zdq5XE5yOBwvuKoNxGfBkQ8FzXkPprnj9Xrfm82mDI8fsLON3x5H/Od+RwHdLfDds9vtn0aj8QoF6QH9JzjuG3acpxmu1RgPAAAAAElFTkSuQmCC";
        private const string s_Rotated = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPCAYAAAA71pVKAAAABGdBTUEAALGPC/xhBQAAAAlwSFlzAAAOwQAADsEBuJFr7QAAABh0RVh0U29mdHdhcmUAcGFpbnQubmV0IDQuMC41ZYUyZQAAAHdJREFUOE+djssNwCAMQxmIFdgx+2S4Vj4YxWlQgcOT8nuG5u5C732Sd3lfLlmPMR4QhXgrTQaimUlA3EtD+CJlBuQ7aUAUMjEAv9gWCQNEPhHJUkYfZ1kEpcxDzioRzGIlr0Qwi0r+Q5rTgM+AAVcygHgt7+HtBZs/2QVWP8ahAAAAAElFTkSuQmCC";
        private const string s_Fixed = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPCAYAAAA71pVKAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAZdEVYdFNvZnR3YXJlAHBhaW50Lm5ldCA0LjAuMjHxIGmVAAAA50lEQVQ4T51Ruw6CQBCkwBYKWkIgQAs9gfgCvgb4BML/qWBM9Bdo9QPIuVOQ3JIzosVkc7Mzty9NCPE3lORaKMm1YA/LsnTXdbdhGJ6iKHoVRTEi+r4/OI6zN01Tl/XM7HneLsuyW13XU9u2ous6gYh3kiR327YPsp6ZgyDom6aZYFqiqqqJ8mdZz8xoca64BHjkZT0zY0aVcQbysp6Z4zj+Vvkp65mZttxjOSozdkEzD7KemekcxzRNHxDOHSDiQ/DIy3pmpjtuSJBThStGKMtyRKSOLnSm3DCMz3f+FUpyLZTkOgjtDSWORSDbpbmNAAAAAElFTkSuQmCC";

        private Texture2D[] arrows;
        private Texture2D[] arrowsIcons {
            get {
                if (arrows == null) {
                    arrows = new Texture2D[10];
                    arrows[0] = Base64ToTexture(s_Arrow0);
                    arrows[1] = Base64ToTexture(s_Arrow1);
                    arrows[2] = Base64ToTexture(s_Arrow2);
                    arrows[3] = Base64ToTexture(s_Arrow3);
                    arrows[5] = Base64ToTexture(s_Arrow5);
                    arrows[6] = Base64ToTexture(s_Arrow6);
                    arrows[7] = Base64ToTexture(s_Arrow7);
                    arrows[8] = Base64ToTexture(s_Arrow8);
                    arrows[9] = Base64ToTexture(s_XIconString);
                }

                return arrows;
            }
        }

        #region @CLASSROOM: Sobre `OnEnable()`.
        /**
         * Quando o Asset é clicado, selecionado.
         */
        #endregion
        private void OnEnable() {
            neighborRuleCodes = EnumRuleCodeToListRuleCode();

            CreateReorderableListOfAllRules();
        }

        #region @CLASSROOM: Sobre `OnDisable()`.
        /**
         * Quando o Asset perde o foco. Quando outro Asset é clicado, selecionado.
         */
        #endregion
        private void OnDisable() {
        }

        #region Editor override methods implementation: The start execution.
        public override void OnInspectorGUI() {
            #region @CLASSROOM: `BeginChangeCheck()` e `EndChangeCheck()`.
            /**
             * Os métodos `BeginChangeCheck()` e `EndChangeCheck()` verificam se algum dos 'Fields' abaixo foi modificado.
             * Assim que qualquer 'Field' é alterado, o método `EndChangeCheck()` é executado.
             */
            #endregion
            EditorGUI.BeginChangeCheck();

            // Draw default fields.
            scriptedTile.defaultTileSprite = EditorGUILayout.ObjectField("Default Sprite", scriptedTile.defaultTileSprite, typeof(Sprite), false) as Sprite;
            scriptedTile.defaultTileGameObject = EditorGUILayout.ObjectField("Default Game Object", scriptedTile.defaultTileGameObject, typeof(GameObject), false) as GameObject;
            scriptedTile.defaultTileColliderType = (Tile.ColliderType)EditorGUILayout.EnumPopup("Default Collider Type", scriptedTile.defaultTileColliderType);

            EditorGUILayout.Space(20);

            // Draw ReorderableList of all Rules.
            if (reorderableListOfAllRules != null) {
                reorderableListOfAllRules.DoLayoutList();
            }

            if (EditorGUI.EndChangeCheck()) {
                ForceRefreshTileOfTileBase();
            }
        }

        public override bool HasPreviewGUI() {
            return base.HasPreviewGUI();
        }

        public override void OnPreviewGUI(Rect rect, GUIStyle background) {
        }

        //public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height) {
        //    return base.RenderStaticPreview(assetPath, subAssets, width, height);
        //}
        #endregion

        #region ReorderableList methods: The Rules.
        private void CreateReorderableListOfAllRules() {
            bool draggable = true;
            bool displayHeader = true;
            bool displayAddButton = true;
            bool displayRemoveButton = true;

            reorderableListOfAllRules = new ReorderableList(
                scriptedTile.rules,
                typeof(ScriptedTile.Rule),
                draggable,
                displayHeader,
                displayAddButton,
                displayRemoveButton
            );

            reorderableListOfAllRules.drawHeaderCallback = OnDrawReorderableListHeader;
            reorderableListOfAllRules.drawElementCallback = OnDrawReorderableListElement;
            reorderableListOfAllRules.elementHeightCallback = GetReorderableListElementHeight;
            reorderableListOfAllRules.onChangedCallback = OnReorderableListUpdated;
            reorderableListOfAllRules.onAddCallback = OnAddElementInReorderableList;
        }

        private void OnDrawReorderableListHeader(Rect rectangle) {
            GUI.Label(rectangle, "Rules");
        }

        /**
         * Para cada Rule, este método será executado.
         * 
         * Em cada elemento tem 3 componentes: Tile data, Matrix e Sprite.
         */
        private void OnDrawReorderableListElement(Rect elementRectangle, int ruleIndex, bool isActive, bool isFocused) {
            ScriptedTile.Rule rule = scriptedTile.rules[ruleIndex];

            float elementStartPositionX = elementRectangle.xMin;
            float elementStartPositionY = elementRectangle.yMin;
            float elementMaxWidth = elementRectangle.xMax;

            BoundsInt neighborsMatrixBounds = GetNeighborsMatrixBounds(rule.GetNeighborsMatrixBoundsBySelectedNeighbors());
            Vector2 neighborsMatrixGUI = TransformNeighborsMatrixBoundsToGUI(neighborsMatrixBounds);

            float neighborsMatrixGUIWidth = neighborsMatrixGUI.x;
            float neighborsMatrixGUIHeight = neighborsMatrixGUI.y;

            #region SpriteComponent Rectangle.
            float spriteComponentPositionX = elementMaxWidth - ReorderableListGUIDefaults.ComponentWidth;
            float spriteComponentPositionY = elementStartPositionY + ReorderableListGUIDefaults.FieldPaddingTop * 2;
            float spriteComponentWidth = ReorderableListGUIDefaults.ComponentWidth;
            float spriteComponentHeight = ReorderableListGUIDefaults.ElementHeight;

            // Separar o espaço deste componente dentro do element do ReorderableList.
            Rect spriteComponentRectangle = new Rect(
                spriteComponentPositionX,
                spriteComponentPositionY,
                spriteComponentWidth,
                spriteComponentHeight
            );
            #endregion

            #region NeighborsMatrixComponent Rectangle.
            float neighborsMatrixComponentPositionX = elementMaxWidth - neighborsMatrixGUIWidth - spriteComponentWidth - 10f;
            float neighborsMatrixComponentPositionY = elementStartPositionY + ReorderableListGUIDefaults.FieldPaddingTop * 2;
            float neighborsMatrixComponentWidth = neighborsMatrixGUIWidth;
            float neighborsMatrixComponentHeight = neighborsMatrixGUIHeight;

            // Separar o espaço deste componente dentro do element do ReorderableList.
            Rect neighborsMatrixComponentRectangle = new Rect(
                neighborsMatrixComponentPositionX,
                neighborsMatrixComponentPositionY,
                neighborsMatrixComponentWidth,
                neighborsMatrixComponentHeight
            );
            #endregion

            #region TileDataComponent Rectangle.
            float tileDataComponentPositionX = elementStartPositionX;
            float tileDataComponentPositionY = elementStartPositionY + ReorderableListGUIDefaults.FieldPaddingTop;
            float tileDataComponentWidth = elementRectangle.width - neighborsMatrixGUIWidth - spriteComponentRectangle.width - 20f;
            float tileDataComponentHeight = elementRectangle.height - ReorderableListGUIDefaults.ElementPaddingHeight;

            // Separar o espaço deste componente dentro do element do ReorderableList.
            Rect tileDataComponentRectangle = new Rect(
                tileDataComponentPositionX,
                tileDataComponentPositionY,
                tileDataComponentWidth,
                tileDataComponentHeight
            );
            #endregion

            DrawTileDataComponent(tileDataComponentRectangle, rule);
            DrawNeighborsMatrixComponent(neighborsMatrixComponentRectangle, neighborsMatrixBounds, scriptedTile, rule);
            DrawSpriteComponent(spriteComponentRectangle, rule);
        }

        #region ReorderableList element draws components.
        private void DrawSpriteComponent(Rect spriteComponentRectangle, ScriptedTile.Rule rule) {
            rule.tileSprites[0] = EditorGUI.ObjectField(spriteComponentRectangle, rule.tileSprites[0], typeof(Sprite), false) as Sprite;
        }

        private void DrawNeighborsMatrixComponent(Rect neighborsMatrixComponentRectangle, BoundsInt neighborsMatrixBounds, ScriptedTile scriptedTile, ScriptedTile.Rule rule) {
            Handles.color = EditorGUIUtility.isProSkin ?
                new Color(1f, 1f, 1f, 0.2f) :
                new Color(0f, 0f, 0f, 0.2f);

            /*
             * Regras de substituição, Neighbors.
             * 
             * Neighbors Matrix é o conjunto de Neighbor boxes.
             */

            // Largura do Matrix GUI.
            float matrixWidth = neighborsMatrixComponentRectangle.width;
            // Altura do Matrix GUI.
            float matrixHeight = neighborsMatrixComponentRectangle.height;

            // Quantidade de colunas verticais do Matrix.
            float boundsWidth = neighborsMatrixBounds.size.x;
            // Quantidade de colunas horizontais do Matrix.
            float boundsHeight = neighborsMatrixBounds.size.y;

            int boundsMaxX = neighborsMatrixBounds.xMax;
            int boundsMaxY = neighborsMatrixBounds.yMax;
            int boundsMinX = neighborsMatrixBounds.xMin;
            int boundsMinY = neighborsMatrixBounds.yMin;

            // Largura de cada coluna do Matrix.
            float matrixColumnWidth = matrixWidth / boundsWidth;
            // Altura de cada coluna do Matrix.
            float matrixColumnHeight = matrixHeight / boundsHeight;

            float matrixGUITop = neighborsMatrixComponentRectangle.yMin;
            float matrixGUIRight = neighborsMatrixComponentRectangle.xMax;
            float matrixGUIBottom = neighborsMatrixComponentRectangle.yMax;
            float matrixGUILeft = neighborsMatrixComponentRectangle.xMin;
            
            // Draw horizontal columns.
            for (int horizontalColumnIndex = 0; horizontalColumnIndex <= boundsHeight; horizontalColumnIndex++) {
                float columnHeight = matrixGUITop + horizontalColumnIndex * matrixColumnHeight;

                Vector3 startPoint = new Vector3(matrixGUILeft, columnHeight);
                Vector3 endPoint = new Vector3(matrixGUIRight, columnHeight);

                Handles.DrawLine(startPoint, endPoint);
            }

            // Draw vertical columns.
            for (int verticalColumnIndex = 0; verticalColumnIndex <= boundsWidth; verticalColumnIndex++) {
                float columnWidth = matrixGUILeft + verticalColumnIndex * matrixColumnWidth;

                Vector3 startPoint = new Vector3(columnWidth, matrixGUITop);
                Vector3 endPoint = new Vector3(columnWidth, matrixGUIBottom);

                Handles.DrawLine(startPoint, endPoint);
            }

            Handles.color = Color.white;

            var neighbors = rule.GetNeighbors();

            for (int neighborY = boundsMinY; neighborY < boundsMaxX; neighborY++) {
                for (int neighborX = boundsMinX; neighborX < boundsMaxY; neighborX++) {
                    Vector3Int neighborBoxPositionOnMatrix = new Vector3Int(neighborX, neighborY, 0);

                    float neighborBoxPositionX = matrixGUILeft + (neighborX - boundsMinX) * matrixColumnWidth;
                    float neighborBoxPositionY = matrixGUITop + (-neighborY + boundsMaxY - 1) * matrixColumnHeight;
                    float width = matrixColumnWidth - 1;
                    float height = matrixColumnHeight - 1;

                    Rect neighborBoxRectangle = new Rect(
                        neighborBoxPositionX,
                        neighborBoxPositionY,
                        width,
                        height
                    );

                    using (var changeCheckScope = new EditorGUI.ChangeCheckScope()) {
                        if (neighborBoxPositionOnMatrix.x != 0 || neighborBoxPositionOnMatrix.y != 0) {
                            if (neighbors.ContainsKey(neighborBoxPositionOnMatrix)) {
                                DrawIcons(neighborBoxRectangle, neighborBoxPositionOnMatrix, neighbors[neighborBoxPositionOnMatrix]);
                                // Tooltip
                            }

                            OnNeighborClick(neighborBoxRectangle, rule, neighbors, neighborBoxPositionOnMatrix);
                        } else {

                        }
                    }
                }
            }
        }

        private int GetArrowIconIndex(Vector3Int neighborBoxPositionOnMatrix) {
            if (Mathf.Abs(neighborBoxPositionOnMatrix.x) == Mathf.Abs(neighborBoxPositionOnMatrix.y)) {
                if (neighborBoxPositionOnMatrix.x < 0 && neighborBoxPositionOnMatrix.y > 0) {
                    return 0;
                } else if (neighborBoxPositionOnMatrix.x > 0 && neighborBoxPositionOnMatrix.y > 0) {
                    return 2;
                } else if (neighborBoxPositionOnMatrix.x < 0 && neighborBoxPositionOnMatrix.y < 0) {
                    return 6;
                } else if (neighborBoxPositionOnMatrix.x > 0 && neighborBoxPositionOnMatrix.y < 0) {
                    return 8;
                }
            } else if (Mathf.Abs(neighborBoxPositionOnMatrix.x) > Mathf.Abs(neighborBoxPositionOnMatrix.y)) {
                if (neighborBoxPositionOnMatrix.x > 0) {
                    return 5;
                } else {
                    return 3;
                }
            } else {
                if (neighborBoxPositionOnMatrix.y > 0) {
                    return 1;
                } else {
                    return 7;
                }
            }

            return -1;
        }

        private void DrawIcons(Rect neighborBoxRectangle, Vector3Int neighborBoxPositionOnMatrix, int ruleCode) {
            switch (ruleCode) {
                case ScriptedTile.RuleTileData.NeighborRuleCode.MustHave:
                    GUI.DrawTexture(neighborBoxRectangle, arrowsIcons[GetArrowIconIndex(neighborBoxPositionOnMatrix)]);
                    break;
                case ScriptedTile.RuleTileData.NeighborRuleCode.MustNotHave:
                    GUI.DrawTexture(neighborBoxRectangle, arrowsIcons[9]);
                    break;
                default:
                    GUIStyle style = new GUIStyle();
                    style.alignment = TextAnchor.MiddleCenter;
                    style.fontSize = 10;
                    GUI.Label(neighborBoxRectangle, ruleCode.ToString(), style);
                    break;
            }
        }

        private void ByNeighbor(ScriptedTile.Rule rule) {
            
        }

        private void OnNeighborClick(Rect neighborBoxRectangle, ScriptedTile.Rule rule, Dictionary<Vector3Int, int> neighbors, Vector3Int neighborBoxPositionOnMatrix) {
            if (Event.current.type == EventType.MouseDown && neighborBoxRectangle.Contains(Event.current.mousePosition)) {
                // O clique vai avançar ou voltar Rule Code de um NeighborBox.
                int getChangedClick = Event.current.button == 1 ? -1 : 1;

                // Se já houver um Neighbor associado a um NeighborBox.
                if (neighbors.ContainsKey(neighborBoxPositionOnMatrix)) {
                    int currentRuleCode = neighborRuleCodes.IndexOf(neighbors[neighborBoxPositionOnMatrix]);
                    int changedRuleCode = currentRuleCode + getChangedClick;

                    if (changedRuleCode >= 0 && changedRuleCode < neighborRuleCodes.Count) {
                        changedRuleCode = (int)Mathf.Repeat(changedRuleCode, neighborRuleCodes.Count);
                        neighbors[neighborBoxPositionOnMatrix] = neighborRuleCodes[changedRuleCode];
                    } else {
                        neighbors.Remove(neighborBoxPositionOnMatrix);
                    }
                } else {
                    // Se não houver um Neighbor associado a um NeighborBox.
                    neighbors.Add(neighborBoxPositionOnMatrix, neighborRuleCodes[getChangedClick == 1 ? 0 : (neighborRuleCodes.Count - 1)]);
                }

                rule.SetNeighbors(neighbors);

                GUI.changed = true;
                Event.current.Use();
            }
        }

        private List<int> EnumRuleCodeToListRuleCode() {
            FieldInfo[] getNeighborRuleCodes = scriptedTile.neighborRuleCodes.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

            Func<FieldInfo, int> selectorRuleCodes = ruleCode => (int)ruleCode.GetValue(null);
            List<int> neighborRuleCodes = getNeighborRuleCodes.Select(selectorRuleCodes).ToList();
            neighborRuleCodes.Sort();

            return neighborRuleCodes;
        }

        private void DrawTileDataComponent(Rect tileDataComponentRectangle, ScriptedTile.Rule rule) {
            float componentPositionX = tileDataComponentRectangle.x;
            float fieldPositionY = tileDataComponentRectangle.y;

            /*
             * Inicia o `fieldOrderNumber`.
             * Ele controla a quantidade de padding que cada Field precisa.
             * A quantidade de padding é baseada na ordem (1º, 2º, ...) do Field.
             */
            int fieldOrderNumber = UpdateFieldOrderNumber(0);

            #region GameObject Field.
            Rect gameObjectLabelRectangle = new Rect(
                componentPositionX,
                fieldPositionY + GetFieldPaddingTopByFieldPositionY(fieldPositionY, fieldOrderNumber),
                ReorderableListGUIDefaults.FieldWidth,
                ReorderableListGUIDefaults.FieldHeight
            );

            Rect gameObjectFieldRectangle = new Rect(
                componentPositionX + ReorderableListGUIDefaults.FieldWidth,
                fieldPositionY + GetFieldPaddingTopByFieldPositionY(fieldPositionY, fieldOrderNumber),
                /*
                 * `componentRectangle.width`: A largura do Field é proporcional a largura do element do ReorderableList,
                 * pois, `componentRectangle` é baseado no elementRectangle.
                 */
                tileDataComponentRectangle.width - ReorderableListGUIDefaults.FieldWidth,
                ReorderableListGUIDefaults.FieldHeight
            );

            GUI.Label(gameObjectLabelRectangle, "Game Object");
            rule.tileGameObject = EditorGUI.ObjectField(gameObjectFieldRectangle, "", rule.tileGameObject, typeof(GameObject), false) as GameObject;

            fieldPositionY = UpdateFieldPositionY(fieldPositionY);
            fieldOrderNumber = UpdateFieldOrderNumber(fieldOrderNumber);
            #endregion

            #region ColliderType Field.
            Rect colliderTypeLabelRectangle = new Rect(
                componentPositionX,
                fieldPositionY + GetFieldPaddingTopByFieldPositionY(fieldPositionY, fieldOrderNumber),
                ReorderableListGUIDefaults.FieldWidth,
                ReorderableListGUIDefaults.FieldHeight
            );

            Rect colliderTypeFieldRectangle = new Rect(
                componentPositionX + ReorderableListGUIDefaults.FieldWidth,
                fieldPositionY + GetFieldPaddingTopByFieldPositionY(fieldPositionY, fieldOrderNumber),
                /*
                 * `componentRectangle.width`: A largura do Field é proporcional a largura do element do ReorderableList,
                 * pois, `componentRectangle` é baseado no elementRectangle.
                 */
                tileDataComponentRectangle.width - ReorderableListGUIDefaults.FieldWidth,
                ReorderableListGUIDefaults.FieldHeight
            );

            GUI.Label(colliderTypeLabelRectangle, "Collider Type");
            rule.tileColliderType = (Tile.ColliderType)EditorGUI.EnumPopup(colliderTypeFieldRectangle, rule.tileColliderType);

            fieldPositionY = UpdateFieldPositionY(fieldPositionY);
            fieldOrderNumber = UpdateFieldOrderNumber(fieldOrderNumber);
            #endregion

            #region SpriteOutputType Field.
            Rect spriteOutputTypeLabelRectangle = new Rect(
                componentPositionX,
                fieldPositionY + GetFieldPaddingTopByFieldPositionY(fieldPositionY, fieldOrderNumber),
                ReorderableListGUIDefaults.FieldWidth,
                ReorderableListGUIDefaults.FieldHeight
            );

            Rect spriteOutputTypeFieldRectangle = new Rect(
                componentPositionX + ReorderableListGUIDefaults.FieldWidth,
                fieldPositionY + GetFieldPaddingTopByFieldPositionY(fieldPositionY, fieldOrderNumber),
                /*
                 * `componentRectangle.width`: A largura do Field é proporcional a largura do element do ReorderableList,
                 * pois, `componentRectangle` é baseado no elementRectangle.
                 */
                tileDataComponentRectangle.width - ReorderableListGUIDefaults.FieldWidth,
                ReorderableListGUIDefaults.FieldHeight
            );

            GUI.Label(spriteOutputTypeLabelRectangle, "Sprite Output");
            rule.spriteOutputType = (ScriptedTile.RuleTileData.SpriteOutputType)EditorGUI.EnumPopup(spriteOutputTypeFieldRectangle, rule.spriteOutputType);

            fieldPositionY = UpdateFieldPositionY(fieldPositionY);
            fieldOrderNumber = UpdateFieldOrderNumber(fieldOrderNumber);
            #endregion

            if (rule.spriteOutputType == ScriptedTile.RuleTileData.SpriteOutputType.Animation) {
                #region AnimationSpeed Field.
                Rect animationSpeedLabelRectangle = new Rect(
                    componentPositionX,
                    fieldPositionY + GetFieldPaddingTopByFieldPositionY(fieldPositionY, fieldOrderNumber),
                    ReorderableListGUIDefaults.FieldWidth,
                    ReorderableListGUIDefaults.FieldHeight
                );

                Rect animationSpeedFieldRectangle = new Rect(
                    componentPositionX + ReorderableListGUIDefaults.FieldWidth,
                    fieldPositionY + GetFieldPaddingTopByFieldPositionY(fieldPositionY, fieldOrderNumber),
                    /*
                     * `componentRectangle.width`: A largura do Field é proporcional a largura do element do ReorderableList,
                     * pois, `componentRectangle` é baseado no elementRectangle.
                     */
                    tileDataComponentRectangle.width - ReorderableListGUIDefaults.FieldWidth,
                    ReorderableListGUIDefaults.FieldHeight
                );

                GUI.Label(animationSpeedLabelRectangle, "Speed");
                rule.animationSpeed = EditorGUI.FloatField(animationSpeedFieldRectangle, rule.animationSpeed);

                fieldPositionY = UpdateFieldPositionY(fieldPositionY);
                fieldOrderNumber = UpdateFieldOrderNumber(fieldOrderNumber);
                #endregion
            }

            if (rule.spriteOutputType == ScriptedTile.RuleTileData.SpriteOutputType.Random) {
                #region PerlinNoise Field.
                Rect animationSpeedLabelRectangle = new Rect(
                    componentPositionX,
                    fieldPositionY + GetFieldPaddingTopByFieldPositionY(fieldPositionY, fieldOrderNumber),
                    ReorderableListGUIDefaults.FieldWidth,
                    ReorderableListGUIDefaults.FieldHeight
                );

                Rect animationSpeedFieldRectangle = new Rect(
                    componentPositionX + ReorderableListGUIDefaults.FieldWidth,
                    fieldPositionY + GetFieldPaddingTopByFieldPositionY(fieldPositionY, fieldOrderNumber),
                    /*
                     * `componentRectangle.width`: A largura do Field é proporcional a largura do element do ReorderableList,
                     * pois, `componentRectangle` é baseado no elementRectangle.
                     */
                    tileDataComponentRectangle.width - ReorderableListGUIDefaults.FieldWidth,
                    ReorderableListGUIDefaults.FieldHeight
                );

                GUI.Label(animationSpeedLabelRectangle, "Perlin Noise");
                rule.perlinNoise = EditorGUI.Slider(animationSpeedFieldRectangle, rule.perlinNoise, 0.001f, 0.999f);

                fieldPositionY = UpdateFieldPositionY(fieldPositionY);
                fieldOrderNumber = UpdateFieldOrderNumber(fieldOrderNumber);
                #endregion

                #region RandomRotationType Field.
                Rect randomRotationTypeLabelRectangle = new Rect(
                    componentPositionX,
                    fieldPositionY + GetFieldPaddingTopByFieldPositionY(fieldPositionY, fieldOrderNumber),
                    ReorderableListGUIDefaults.FieldWidth,
                    ReorderableListGUIDefaults.FieldHeight
                );

                Rect randomRotationTypeFieldRectangle = new Rect(
                    componentPositionX + ReorderableListGUIDefaults.FieldWidth,
                    fieldPositionY + GetFieldPaddingTopByFieldPositionY(fieldPositionY, fieldOrderNumber),
                    /*
                     * `componentRectangle.width`: A largura do Field é proporcional a largura do element do ReorderableList,
                     * pois, `componentRectangle` é baseado no elementRectangle.
                     */
                    tileDataComponentRectangle.width - ReorderableListGUIDefaults.FieldWidth,
                    ReorderableListGUIDefaults.FieldHeight
                );

                GUI.Label(randomRotationTypeLabelRectangle, "Rotation");
                rule.randomRotationType = (ScriptedTile.Rule.RandomRotationType)EditorGUI.EnumPopup(randomRotationTypeFieldRectangle, rule.randomRotationType);

                fieldPositionY = UpdateFieldPositionY(fieldPositionY);
                fieldOrderNumber = UpdateFieldOrderNumber(fieldOrderNumber);
                #endregion
            }

            // Lista de Sprites para a animação ou, para a randomização.
            if (rule.spriteOutputType != ScriptedTile.RuleTileData.SpriteOutputType.Single) {
                #region Sprites Fields.
                Rect spritesNumberLabelRectangle = new Rect(
                    componentPositionX,
                    fieldPositionY + GetFieldPaddingTopByFieldPositionY(fieldPositionY, fieldOrderNumber),
                    ReorderableListGUIDefaults.FieldWidth,
                    ReorderableListGUIDefaults.FieldHeight
                );

                Rect spritesNumberFieldRectangle = new Rect(
                    componentPositionX + ReorderableListGUIDefaults.FieldWidth,
                    fieldPositionY + GetFieldPaddingTopByFieldPositionY(fieldPositionY, fieldOrderNumber),
                    /*
                     * `componentRectangle.width`: A largura do Field é proporcional a largura do element do ReorderableList,
                     * pois, `componentRectangle` é baseado no elementRectangle.
                     */
                    tileDataComponentRectangle.width - ReorderableListGUIDefaults.FieldWidth,
                    ReorderableListGUIDefaults.FieldHeight
                );

                EditorGUI.BeginChangeCheck();

                GUI.Label(spritesNumberLabelRectangle, "Sprites");
                int spritesNumber = EditorGUI.DelayedIntField(spritesNumberFieldRectangle, rule.tileSprites.Length);

                /*
                 * Não há necessidade de ter mais que 500 Sprites por Rule.
                 * Um valor muito grande pode causar atrasos no Editor, por causa da quantidade de loops.
                 */
                spritesNumber = Mathf.Min(500, spritesNumber);

                // Modificar a lista de Sprites quando o Field acima (a quantidade de Sprites) for modificado.
                if (EditorGUI.EndChangeCheck()) {
                    // Deve ter pelo menos um campo.
                    Array.Resize(ref rule.tileSprites, Mathf.Max(spritesNumber, 1));
                }

                fieldPositionY = UpdateFieldPositionY(fieldPositionY);
                fieldOrderNumber = UpdateFieldOrderNumber(fieldOrderNumber);

                for (int spriteIndex = 0; spriteIndex < rule.tileSprites.Length; spriteIndex++) {
                    Rect spriteFieldRectangle = new Rect(
                        componentPositionX + ReorderableListGUIDefaults.FieldWidth,
                        fieldPositionY + GetFieldPaddingTopByFieldPositionY(fieldPositionY, fieldOrderNumber),
                        /*
                         * `componentRectangle.width`: A largura do Field é proporcional a largura do element do ReorderableList,
                         * pois, `componentRectangle` é baseado no elementRectangle.
                         */
                        tileDataComponentRectangle.width - ReorderableListGUIDefaults.FieldWidth,
                        ReorderableListGUIDefaults.FieldHeight
                    );

                    rule.tileSprites[spriteIndex] = EditorGUI.ObjectField(spriteFieldRectangle, rule.tileSprites[spriteIndex], typeof(Sprite), false) as Sprite;

                    fieldPositionY = UpdateFieldPositionY(fieldPositionY);
                    fieldOrderNumber = UpdateFieldOrderNumber(fieldOrderNumber);
                }
                #endregion
            }
        }
        #endregion

        private float GetReorderableListElementHeight(int ruleIndex) {
            ScriptedTile.Rule rule = scriptedTile.rules[ruleIndex];

            BoundsInt neighborsMatrixBounds = GetNeighborsMatrixBounds(rule.GetNeighborsMatrixBoundsBySelectedNeighbors());

            float heightByTileDataComponent = 0f;

            float sumOfFieldPaddingTop;
            float sumOfheightOfAllSpritesFields;

            switch (rule.spriteOutputType) {
                case ScriptedTile.RuleTileData.SpriteOutputType.Single:
                    heightByTileDataComponent = ReorderableListGUIDefaults.ElementHeight + ReorderableListGUIDefaults.ElementPaddingHeight + ReorderableListGUIDefaults.FieldPaddingTop * 3;
                    break;

                case ScriptedTile.RuleTileData.SpriteOutputType.Random:
                    // Iniciar com 6 Fields.
                    sumOfFieldPaddingTop = ReorderableListGUIDefaults.FieldPaddingTop * 6;

                    for (int i = 0; i < rule.tileSprites.Length; i++) {
                        sumOfFieldPaddingTop += ReorderableListGUIDefaults.FieldPaddingTop;
                    }

                    sumOfheightOfAllSpritesFields = ReorderableListGUIDefaults.FieldHeight * (rule.tileSprites.Length + 3);

                    heightByTileDataComponent = sumOfFieldPaddingTop + sumOfheightOfAllSpritesFields + ReorderableListGUIDefaults.ElementHeight + ReorderableListGUIDefaults.ElementPaddingHeight;
                    break;

                case ScriptedTile.RuleTileData.SpriteOutputType.Animation:
                    // Iniciar com 5 Fields.
                    sumOfFieldPaddingTop = ReorderableListGUIDefaults.FieldPaddingTop * 5;

                    for (int i = 0; i < rule.tileSprites.Length; i++) {
                        sumOfFieldPaddingTop += ReorderableListGUIDefaults.FieldPaddingTop;
                    }

                    sumOfheightOfAllSpritesFields = ReorderableListGUIDefaults.FieldHeight * (rule.tileSprites.Length + 2);

                    heightByTileDataComponent = sumOfFieldPaddingTop + sumOfheightOfAllSpritesFields + ReorderableListGUIDefaults.ElementHeight + ReorderableListGUIDefaults.ElementPaddingHeight;
                    break;
            }

            float neighborsMatrixGUIHeight = TransformNeighborsMatrixBoundsToGUI(neighborsMatrixBounds).y + 10f;

            // Qual componente tem a maior altura? TileDataComponent ou NeighborMatrixBounds.
            return Mathf.Max(heightByTileDataComponent, neighborsMatrixGUIHeight);
        }

        private void OnReorderableListUpdated(ReorderableList list) { }

        /**
         * Ao adicionar uma nova Rule na lista de Rule: `ScriptedTile.rules`.
         * A `reorderableListOfAllRules` usa esta lista para renderizar cada Rule no Inspector.
         */
        private void OnAddElementInReorderableList(ReorderableList list) {
            ScriptedTile.Rule rule = new ScriptedTile.Rule();

            // Use default values.
            rule.tileSprites[0] = scriptedTile.defaultTileSprite;
            rule.tileGameObject = scriptedTile.defaultTileGameObject;
            rule.tileColliderType = scriptedTile.defaultTileColliderType;
            rule.spriteOutputType = ScriptedTile.RuleTileData.SpriteOutputType.Single;

            scriptedTile.rules.Add(rule);
        }
        #endregion

        private void ForceRefreshTileOfTileBase() {
            /**
             * This method force the `TileBase.RefreshTile()`.
             * Then the `ScriptedTile.GetTileData()` will be executed for all Tiles.
             */
            EditorUtility.SetDirty(target);
            SceneView.RepaintAll();
        }

        private BoundsInt GetNeighborsMatrixBounds(BoundsInt bounds) {
            /*
             * BoundsInt, basicamente é um plano cartesiano.
             * Tem o menor ponto no eixo X: `xMin`.
             * Tem o maior ponto no eixo X: `xMax`.
             * Tem a largura total: `size.x` = |`xMin` + `xMax`|
             * 
             * Tem o menor ponto no eixo Y: `yMin`.
             * Tem o maior ponto no eixo Y: `yMax`.
             * Tem a altura total: `size.y` = |`yMin` + `yMax`|
             */

            bounds.xMin = Mathf.Min(bounds.xMin, -1);
            bounds.yMin = Mathf.Min(bounds.yMin, -1);
            bounds.xMax = Mathf.Max(bounds.xMax, 2);
            bounds.yMax = Mathf.Max(bounds.yMax, 2);

            return bounds;
        }

        private Vector2 TransformNeighborsMatrixBoundsToGUI(BoundsInt neighborsMatrixBounds) {
            int neighborsMatrixBoundsWidth = neighborsMatrixBounds.size.x;
            int neighborsMatrixBoundsHeight = neighborsMatrixBounds.size.y;

            return new Vector2(
                neighborsMatrixBoundsWidth * ReorderableListGUIDefaults.FieldHeight,
                neighborsMatrixBoundsHeight * ReorderableListGUIDefaults.FieldHeight
            );
        }

        private float GetFieldPaddingTopByFieldPositionY(float fieldPositionY, int fieldOrderNumber) {
            if (fieldOrderNumber == 1 || fieldOrderNumber == 2) {
                return ReorderableListGUIDefaults.FieldPaddingTop * fieldOrderNumber;
            }

            int fieldOrderNumberMultiplier = 2;

            for (int i = 3; i <= fieldOrderNumber; i++) {
                fieldOrderNumberMultiplier += 1;
            }

            return ReorderableListGUIDefaults.FieldPaddingTop * fieldOrderNumberMultiplier;
        }

        private int UpdateFieldOrderNumber(int fieldOrderNumber) {
            return fieldOrderNumber += 1;
        }

        private float UpdateFieldPositionY(float fieldPositionY) {
            return fieldPositionY += ReorderableListGUIDefaults.FieldHeight;
        }

        public static Texture2D Base64ToTexture(string base64) {
            Texture2D texture = new Texture2D(1, 1);
            texture.hideFlags = HideFlags.HideAndDontSave;
            texture.LoadImage(System.Convert.FromBase64String(base64));

            return texture;
        }
    }
}