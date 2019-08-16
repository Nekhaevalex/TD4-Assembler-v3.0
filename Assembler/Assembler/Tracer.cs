using Opcode;
using System;

namespace Assembler
{
    class Tracer
    {

        enum PointType : byte
        {
            waypoint, start, end, bridge, switchPage, empty, fixed_point, old_fixed_point, boundary_point, empty_nop
        }
        class point : ICloneable
        {
            public PointType type;
            public int row;
            public int column;
            public int depth;
            public IOpcode meta;
            public bool toBeDeleted;
            public point target;
            public point owner;
            public bool isStatic;
            public bool isAnalyzed;
            public bool isUserPlaced;
            public point(PointType type, int x, int y, int depth, IOpcode meta, bool toBeDeleted, bool userPlaced)
            {
                this.type = type;
                this.row = x;
                this.column = y;
                this.depth = depth;
                this.meta = meta;
                this.toBeDeleted = toBeDeleted;
                this.target = null;
                this.owner = null;
                this.isStatic = false;
                this.isAnalyzed = false;
                isUserPlaced = userPlaced;
            }

            private point(PointType type, int row, int column, int depth, IOpcode meta, bool toBeDeleted, point target, point owner, bool isStatic, bool isAnalyzed, bool isUserPlaced)
            {
                this.type = type;
                this.row = row;
                this.column = column;
                this.depth = depth;
                this.meta = meta;
                this.toBeDeleted = toBeDeleted;
                this.target = target;
                this.owner = owner;
                this.isStatic = isStatic;
                this.isAnalyzed = isAnalyzed;
                this.isUserPlaced = isUserPlaced;
            }

            public void setTarget(point target)
            {
                this.target = target;
            }
            public void setOwner(point target)
            {
                this.owner = target;
            }

            public void makeStatic()
            {
                isStatic = true;
            }

            public object Clone()
            {
                return new point(this.type, this.row, this.column, this.depth, this.meta, this.toBeDeleted, this.target, this.owner, this.isStatic, this.isAnalyzed, this.isUserPlaced);
            }
        }

        internal Binary MakeBinary()
        {
            IOpcode[][] binary;
            binary = new IOpcode[map.GetSize()][];
            for (int i = 0; i < binary.Length; i++)
            {
                binary[i] = new IOpcode[map.GetSize()];
            }
            int size = map.GetSize();
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    binary[i][j] = map.Get(i,j).meta;
                }
                Console.WriteLine();
            }
            return new Binary(binary);
        }

        private Map map;

        public Tracer(IOpcode[][] build)
        {
            point[][] map = new point[build.Length][];
            for (int i = 0; i < build.Length; i++)
            {
                map[i] = new point[build.Length];
            }
            for (int i = 0; i < build.Length; i++)
            {
                for (int j = 0; j < build[i].Length; j++)
                {
                    map[i][j] = new point(PointType.empty, i, j, 0, build[i][j], false, true);
                }
            }
            this.map = new Map(map, build.Length);
        }

        class Map
        {
            private point[][] map;
            private int size;

            public int GetSize()
            {
                return size;
            }
            private void ShiftAndFree(int row, int column)
            {
                int i = size - 1;
                int j = size - 1;

                int nextColumn = column == size - 2 ? 0 : column + 1;
                int nextRow = nextColumn == 0 ? row + 1 : row;
                if (row == 6 && column == 9)
                {
                    Utilities.Utilities.VerbouseOut("DEBUG", "Point reached");
                }
                while (i * (size - 1) + j > (row * (size - 1) + column))
                {
                    if (j != 0)
                    {
                        if (!map[i][j - 1].isStatic)
                        {
                            map[i][j] = (point)map[i][j - 1].Clone();
                            map[i][j].row = i;
                            map[i][j].column = j;
                            if (map[i][j - 1].owner != null)
                                map[i][j - 1].owner.setTarget(map[i][j]);
                            if (map[i][j - 1].target != null)
                                map[i][j - 1].target.setOwner(map[i][j]);
                            if (map[i][j].target != null)
                            {
                                int targetRow = map[i][j].target.row;
                                int targetColumn = map[i][j].target.column;
                                if (((targetRow * (size - 1) + targetColumn) < (row * (size - 1) + column)) && map[i][j].target.isAnalyzed)
                                {
                                    MoveHelperForward(map[i][j].target.row, map[i][j].target.column);
                                }
                            }
                            j--;
                        }
                        else
                        {
                            int k = j - 1;
                            while (map[i][k].isStatic && k > column)
                            {
                                k--;
                            }
                            map[i][j] = (point)map[i][k].Clone();
                            map[i][j].row = i;
                            map[i][j].column = j;
                            if (map[i][j - 1].owner != null)
                                map[i][j - 1].owner.setTarget(map[i][j]);
                            if (map[i][j - 1].target != null)
                                map[i][j - 1].target.setOwner(map[i][j]);
                            if (map[i][j].target != null)
                            {
                                int targetRow = map[i][j].target.row;
                                int targetColumn = map[i][j].target.column;
                                if ((targetRow * (size - 1) + targetColumn) < (row * (size - 1) + column))
                                {
                                    MoveHelperForward(map[i][j].target.row, map[i][j].target.column);
                                }
                            }
                            j = k;
                        }
                    }
                    else if (j == 0)
                    {
                        map[i][j] = (point)map[i - 1][size - 2].Clone();
                        map[i][j].row = i;
                        map[i][j].column = j;
                        if (map[i - 1][size - 2].owner != null)
                            map[i - 1][size - 2].owner.setTarget(map[i][j]);
                        if (map[i - 1][size - 2].target != null)
                            map[i - 1][size - 2].target.setOwner(map[i][j]);
                        if (map[i][j].target != null)
                        {
                            int targetRow = map[i][j].target.row;
                            int targetColumn = map[i][j].target.column;
                            if ((targetRow * (size - 1) + targetColumn) < (row * (size - 1) + column))
                            {
                                MoveHelperForward(map[i][j].target.row, map[i][j].target.column);
                            }
                        }
                        i--;
                        j = 14;
                    }
                }
                map[row][column].meta = new Add("a", "0");
                map[row][column].type = PointType.empty_nop;
                map[row][column].isUserPlaced = false;
            }



            private void MoveHelperForward(int row, int column)
            {
                if (column < size - 1)
                {
                    int nonStaticPointer = column + 1;
                    while (map[row][nonStaticPointer].isStatic)
                    {
                        nonStaticPointer++;
                    }
                    point lst1 = map[row][column];
                    point lst2 = map[row][nonStaticPointer];

                    point cache = (point)map[row][column].Clone();

                    map[row][column].depth = lst2.depth;
                    map[row][column].isAnalyzed = lst2.isAnalyzed;
                    map[row][column].isStatic = lst2.isStatic;
                    map[row][column].meta = lst2.meta;
                    map[row][column].type = lst2.type;

                    map[row][nonStaticPointer].depth = cache.depth;
                    map[row][nonStaticPointer].isAnalyzed = cache.isAnalyzed;
                    map[row][nonStaticPointer].meta = cache.meta;
                    map[row][nonStaticPointer].type = cache.type;

                    if (nonStaticPointer < 15)
                    {
                        point prev1 = lst1.owner;
                        point prev2 = lst2.owner;
                        point next1 = lst1.target;
                        point next2 = lst2.target;
                        if (lst2 == next1)
                        {
                            lst2.setTarget(lst1);
                            lst2.setOwner(prev1);
                            lst1.setTarget(next2);
                            lst1.setOwner(lst2);
                            if (next2 != null)
                                next2.setOwner(lst1);
                            if (lst1.owner != null)
                                prev1.setTarget(lst2);
                        }
                        else if (lst1 == next2)
                        {
                            lst1.setTarget(lst2);
                            lst1.setOwner(prev2);
                            lst2.setTarget(next1);
                            lst2.setOwner(lst1);
                            if (next1 != null)
                                next1.setOwner(lst2);
                            if (lst2.owner != null)
                                prev2.setTarget(lst1);
                        }
                        else
                        {
                            if (lst1.target != null)
                                prev1.setTarget(lst2);
                            lst2.setTarget(next1);
                            if (lst2.owner != null)
                                prev2.setTarget(lst1);
                            lst1.setTarget(next2);
                            lst2.setOwner(prev1);
                            if (next2 != null)
                                next2.setOwner(lst1);
                            lst1.setOwner(prev2);
                            if (next1 != null)
                                next1.setOwner(lst2);
                        }
                    }
                }
            }
            public point Get(int x, int y)
            {
                return map[x][y];
            }
            private void Trace(int bx, int by, int ex, int ey)
            {
                Utilities.Utilities.VerbouseOut("TRACER", "Tracing PC changers...");
                bool targetReached = false;
                int stepDepth = 1;
                map[bx][by].type = PointType.start;
                map[bx][by].depth = 0;
                map[ex][ey].type = PointType.end;
                //Preventing jumps off (sort of)
                if (ey > 12)
                {
                    MakeAShiftForSaving(ex);
                }
                if (by > 12)
                {
                    MakeAShiftForSaving(bx);
                }
                //Pathfinding (we see numbers field)
                int x, y;
                FindPath(ex, ey, ref targetReached, ref stepDepth, out x, out y);
                point LastTraced = map[x][y];
                //Path tracing (we see path)
                TracePath(bx, by, ref stepDepth, ref x, ref y, ref LastTraced);
                //Clearing unused spaces (we see path only)
                ClearUnusedSpaces();
                //Clearing straight lines (we see netpoints)
                ClearStraightLines();
                //Clearing unused spaces && Linking (we see netpoints only)
                ClearUnusedAndLink();
                //Fix straitgh jumps with wrong ways
                FindAndMakeSingleLineJumps();
                //Implementing points
                ImplementPoints();
                //Moving bridges
                MoveBridges();
            }

            private void FindAndMakeSingleLineJumps()
            {
                point pointer;
                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        if (map[i][j].type == PointType.boundary_point && map[i][j].owner == null)
                        {
                            pointer = map[i][j];
                            int initialRow = pointer.row;
                            while (pointer.target != null)
                            {
                                pointer = pointer.target;
                            }
                            if (initialRow == pointer.row)
                            {
                                point routeStart, routeEnd;
                                RemoveAllWaypoints(pointer, out routeStart, out routeEnd);
                            }
                        }
                    }
                }
            }

            private bool CheckTracks()
            {
                bool everythingAlright = true;
                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        if (map[i][j].type == PointType.boundary_point && map[i][j].owner == null)
                        {
                            point pointer = map[i][j];
                            while (pointer.target != null)
                            {
                                //It's start point;
                                if (pointer.target.row == pointer.row)
                                {
                                    //it's swi
                                    pointer = pointer.target;
                                }
                                else if (pointer.target.column == pointer.column)
                                {
                                    //it's jmp
                                    pointer = pointer.target;
                                }
                                else
                                {
                                    //bug happend
                                    Utilities.Utilities.VerbouseOut("TRACER_DEBUGER", "Found bug at point " + pointer.row.ToString() + ", " + pointer.column.ToString(), ConsoleColor.Red);
                                    Utilities.Utilities.VerbouseOut("TRACER_DEBUGER", "Resetting route and retracing", ConsoleColor.Red);
                                    point routeStart, routeEnd;
                                    RemoveAllWaypoints(pointer, out routeStart, out routeEnd);
                                    routeStart.type = PointType.empty;
                                    routeStart.isAnalyzed = false;
                                    routeEnd.type = PointType.empty;
                                    routeEnd.isAnalyzed = false;
                                    everythingAlright = false;
                                    routeStart.type = PointType.start;
                                    routeEnd.type = PointType.end;
                                    Trace(routeStart.row, routeStart.column, routeEnd.row, routeEnd.column);
                                    return everythingAlright;
                                }
                            }
                        }
                    }
                }
                return everythingAlright;
            }

            private void RemoveAllWaypoints(point pointer, out point routeStart, out point routeEnd)
            {
                routeStart = pointer;
                routeEnd = pointer;
                while (routeStart.owner != null)
                {
                    routeStart = routeStart.owner;
                }
                while (routeEnd.target != null)
                {
                    routeEnd = routeEnd.target;
                }
                point deleter = routeStart.target;
                while (deleter != routeEnd)
                {
                    deleter.owner.setTarget(deleter.target);
                    deleter.target.setOwner(deleter.owner);
                    deleter.type = PointType.empty;
                    deleter.isUserPlaced = false;
                    deleter.meta = new Add("a", "0");
                    map[deleter.row][deleter.column - 1].type = PointType.empty;
                    map[deleter.row][deleter.column - 1].meta = new Add("a", "0");
                    deleter = deleter.target;
                }
            }

            private void MoveBridges()
            {
                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        if (map[i][j].type == PointType.bridge)
                        {
                            if (j == 14)
                            {
                                map[i][j].type = PointType.empty;
                                map[i][j].isAnalyzed = false;
                            }
                            else if (map[i][j + 1].type != PointType.old_fixed_point)
                            {
                                MoveHelperForward(i, j);
                            }
                        }
                    }
                }
            }

            private void MakeAShiftForSaving(int row)
            {
                ShiftAndFree(row, size - 3);
                map[row][size - 3].type = PointType.empty;
                ShiftAndFree(row, size - 2);
                map[row][size - 2].type = PointType.empty;
            }

            private void ClearUnusedSpaces()
            {
                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        if (map[i][j].type == PointType.waypoint)
                        {
                            map[i][j].type = PointType.empty;
                            map[i][j].depth = 0;
                        }
                    }
                }
            }

            private void ImplementPoints()
            {
                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        point currentPoint = map[i][j];
                        if ((currentPoint.type == PointType.old_fixed_point || currentPoint.type == PointType.boundary_point) && currentPoint.isAnalyzed == false)
                        {
                            Utilities.Utilities.VerbouseOut("TRACER", "Investigating point: " + currentPoint.column.ToString() + "," + currentPoint.row.ToString());
                            if (currentPoint.owner == null)
                            {
                                //Its start point
                                //If it's JMP, we can replace it with SWM sometimes:
                                if (currentPoint.meta is Jmp && currentPoint.target.column == currentPoint.column)
                                {
                                    currentPoint.isAnalyzed = true;
                                    Utilities.Utilities.VerbouseOut("TRACER_ANALYZER", "Its JMP and it's target is in the same column: " + currentPoint.target.row.ToString());
                                    currentPoint.meta = new Swi(currentPoint.target.row);
                                    Utilities.Utilities.VerbouseOut("TRACER_ANALYZER", "JMP was replaced with SWM");
                                }
                                /*
                                 * If it's JNC and it's target is not in the same row we must insert construction:
                                 *      JNC (to new help point) <|JMP (over the help point or so called 'bridge'), (new help point)|> ...
                                 * Also we must move target point to the right as it must be above 
                                 */
                                if (currentPoint.meta is Jnc && currentPoint.target.row != currentPoint.row)
                                {
                                    Utilities.Utilities.VerbouseOut("TRACER_ANALYZER", "Its JNC and it's target is in other column: making additional helping point");
                                    int row = currentPoint.row;
                                    int column = currentPoint.column;
                                    Utilities.Utilities.VerbouseOut("TRACER_ANALYZER", "Allocating free space...");
                                    ShiftAndFree(row, column + 1);
                                    Utilities.Utilities.VerbouseOut("TRACER_ANALYZER", "Done");
                                    Utilities.Utilities.VerbouseOut("TRACER_ANALYZER", "Making bridge in " + (column + 1).ToString() + ":" + row.ToString());
                                    map[row][column + 1].meta = new Jmp(column + 2);
                                    map[row][column + 1].type = PointType.bridge;
                                    map[row][column + 1].isAnalyzed = true;
                                    map[row][column + 1].isUserPlaced = false;
                                    map[row][column + 1].column = column + 1;
                                    map[row][column + 1].row = row;
                                    Utilities.Utilities.VerbouseOut("TRACER_ANALYZER", "Allocating free space...");
                                    ShiftAndFree(row, column + 2);
                                    Utilities.Utilities.VerbouseOut("TRACER_ANALYZER", "Done");
                                    Utilities.Utilities.VerbouseOut("TRACER_ANALYZER", "Making new helping point");
                                    point temp = map[row][column + 2];
                                    point p = currentPoint.target;
                                    currentPoint.setTarget(temp);

                                    //value
                                    map[row][column + 2].type = PointType.old_fixed_point;
                                    map[row][column + 2].isAnalyzed = true;
                                    map[row][column].isAnalyzed = true;
                                    map[row][column + 2].column = column + 2;
                                    map[row][column + 2].row = row;

                                    temp.setTarget(p);
                                    temp.setOwner(currentPoint);
                                    if (p != null)
                                    {
                                        p.setOwner(temp);
                                    }
                                    MoveHelperForward(map[row][column + 2].target.row, map[row][column + 2].target.column);
                                    MoveHelperForward(map[row][column + 2].target.row, map[row][column + 2].target.column);
                                }
                            }
                            else if (currentPoint.target == null)
                            {
                                //Its end point
                                Utilities.Utilities.VerbouseOut("TRACER_ANALYZER", "It's end point: nothing to do with it");
                                currentPoint.isAnalyzed = true;
                            }
                            else
                            {
                                //Its help point
                                Utilities.Utilities.VerbouseOut("TRACER_ANALYZER", "It's help point: free and place static");
                                int row = currentPoint.row;
                                int column = currentPoint.column;
                                ShiftAndFree(row, column + 1);
                                //Make free point a waypoint
                                map[row][column].type = PointType.empty;
                                map[row][column + 1].type = PointType.old_fixed_point;
                                map[row][column + 1].setOwner(map[row][column].owner);
                                map[row][column + 1].owner.setTarget(map[row][column + 1]);
                                map[row][column + 1].setTarget(map[row][column].target);
                                map[row][column + 1].target.setOwner(map[row][column + 1]);
                                map[row][column].owner = null;
                                map[row][column].target = null;
                                map[row][column].isAnalyzed = true;
                                ShiftAndFree(row, column + 1);
                                map[row][column + 1].type = PointType.bridge;
                                map[row][column + 1].isAnalyzed = true;
                                map[row][column + 2].isAnalyzed = true;
                            }
                        }
                    }
                }
            }

            private void ClearUnusedAndLink()
            {
                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        if (map[i][j].toBeDeleted && map[i][j].type == PointType.fixed_point)
                        {
                            map[i][j].type = PointType.empty;
                            map[i][j].depth = 0;
                        }
                    }
                }
            }

            private void ClearStraightLines()
            {
                for (int i = 0; i < size - 1; i++)
                {
                    for (int j = 0; j < size - 1; j++)
                    {
                        bool remove = true;
                        if (map[i][j].type == PointType.fixed_point || map[i][j].type == PointType.boundary_point)
                        {
                            if (i > 0)
                            {
                                if (map[i - 1][j].type == PointType.fixed_point || map[i - 1][j].type == PointType.start || map[i - 1][j].type == PointType.end)
                                {
                                    if (j > 0)
                                    {
                                        if (map[i][j - 1].type == PointType.fixed_point || map[i][j - 1].type == PointType.old_fixed_point)
                                        {
                                            remove = false;
                                        }
                                    }
                                    if ((map[i][j + 1].type == PointType.fixed_point || map[i][j + 1].type == PointType.start) || map[i][j + 1].type == PointType.end)
                                    {
                                        remove = false;
                                    }
                                }
                            }
                            if (map[i + 1][j].type == PointType.fixed_point || map[i + 1][j].type == PointType.start || map[i + 1][j].type == PointType.end)
                            {
                                if (j > 0)
                                {
                                    if (map[i][j - 1].type == PointType.fixed_point || map[i][j - 1].type == PointType.old_fixed_point)
                                    {
                                        remove = false;
                                    }
                                }
                                if ((map[i][j + 1].type == PointType.fixed_point || map[i][j + 1].type == PointType.start) || map[i][j + 1].type == PointType.end)
                                {
                                    remove = false;
                                }
                            }
                        }
                        if (map[i][j].type == PointType.start || map[i][j].type == PointType.end)
                        {
                            map[i][j].type = PointType.boundary_point;
                        }
                        if (map[i][j].type == PointType.fixed_point && remove)
                        {
                            map[i][j].toBeDeleted = true;
                            point guyWhoWillBeKilled = map[i][j];
                            if (guyWhoWillBeKilled.owner != null && guyWhoWillBeKilled.target != null)
                            {
                                guyWhoWillBeKilled.target.owner = guyWhoWillBeKilled.owner;
                            }
                            if (guyWhoWillBeKilled.owner != null && guyWhoWillBeKilled.target != null)
                            {
                                guyWhoWillBeKilled.owner.target = guyWhoWillBeKilled.target;
                            }
                            guyWhoWillBeKilled.target = null;
                            guyWhoWillBeKilled.owner = null;
                        }
                        if (map[i][j].type == PointType.fixed_point && !remove)
                        {
                            map[i][j].type = PointType.old_fixed_point;
                        }
                    }
                }
            }

            private void TracePath(int bx, int by, ref int stepDepth, ref int x, ref int y, ref point LastTraced)
            {
                while (map[x][y].type != PointType.start)
                {
                Start:
                    if (y < size - 1)
                    {
                        if ((map[x][y + 1].type == PointType.waypoint || map[x][y + 1].type == PointType.start) && map[x][y + 1].depth == stepDepth)
                        {
                            y += 1;
                            stepDepth--;
                            FixPoint(x, y, LastTraced);
                            if (map[x][y].type != PointType.start)
                                LastTraced = map[x][y];
                            goto Start;
                        }
                    }
                    if (y > 0)
                    {
                        if ((map[x][y - 1].type == PointType.waypoint || map[x][y - 1].type == PointType.start) && map[x][y - 1].depth == stepDepth)
                        {
                            y -= 1;
                            stepDepth--;
                            FixPoint(x, y, LastTraced);
                            if (map[x][y].type != PointType.start)
                                LastTraced = map[x][y];
                            goto Start;
                        }
                    }
                    if (x < size - 1)
                    {
                        if ((map[x + 1][y].type == PointType.waypoint || map[x + 1][y].type == PointType.start) && map[x + 1][y].depth == stepDepth)
                        {
                            x += 1;
                            stepDepth--;
                            FixPoint(x, y, LastTraced);
                            if (map[x][y].type != PointType.start)
                                LastTraced = map[x][y];
                            goto Start;
                        }
                    }
                    if (x > 0)
                    {
                        if ((map[x - 1][y].type == PointType.waypoint || map[x - 1][y].type == PointType.start) && map[x - 1][y].depth == stepDepth)
                        {
                            x -= 1;
                            stepDepth--;
                            FixPoint(x, y, LastTraced);
                            if (map[x][y].type != PointType.start)
                                LastTraced = map[x][y];
                            goto Start;
                        }
                    }
                    stepDepth--;
                }
                map[bx][by].setTarget(LastTraced);
                map[bx][by].setOwner(null);
            }

            private void FindPath(int ex, int ey, ref bool targetReached, ref int stepDepth, out int x, out int y)
            {
                while (!targetReached)
                {
                    for (int i = 0; i < size; i++)
                    {
                        for (int j = 0; j < size; j++)
                        {
                            if (j < size - 1)
                            {
                                if ((map[i][j].type == PointType.waypoint || map[i][j].type == PointType.start) && map[i][j + 1].type == PointType.empty && map[i][j].depth < stepDepth)
                                {
                                    if (j < size - 2)
                                    {
                                        map[i][j + 1].depth = stepDepth;
                                        map[i][j + 1].type = PointType.waypoint;
                                    }
                                }
                                else if ((map[i][j].type == PointType.waypoint || map[i][j].type == PointType.start) && map[i][j + 1].type == PointType.end)
                                {
                                    targetReached = true;
                                    map[i][j + 1].depth = stepDepth;
                                    break;
                                }
                            }
                            if (j > 0)
                            {
                                if ((map[i][j].type == PointType.waypoint || map[i][j].type == PointType.start) && map[i][j - 1].type == PointType.empty && map[i][j].depth < stepDepth)
                                {
                                    map[i][j - 1].depth = stepDepth;
                                    map[i][j - 1].type = PointType.waypoint;
                                }
                                else if ((map[i][j].type == PointType.waypoint || map[i][j].type == PointType.start) && map[i][j - 1].type == PointType.end)
                                {
                                    targetReached = true;
                                    map[i][j - 1].depth = stepDepth;
                                    break;
                                }
                            }
                            if (i < size - 1)
                            {
                                if ((map[i][j].type == PointType.waypoint || map[i][j].type == PointType.start) && map[i + 1][j].type == PointType.empty && map[i][j].depth < stepDepth)
                                {
                                    map[i + 1][j].depth = stepDepth;
                                    map[i + 1][j].type = PointType.waypoint;
                                }
                                else if ((map[i][j].type == PointType.waypoint || map[i][j].type == PointType.start) && map[i + 1][j].type == PointType.end)
                                {
                                    targetReached = true;
                                    map[i + 1][j].depth = stepDepth;
                                    break;
                                }
                            }
                            if (i > 0)
                            {
                                if ((map[i][j].type == PointType.waypoint || map[i][j].type == PointType.start) && map[i - 1][j].type == PointType.empty && map[i][j].depth < stepDepth)
                                {
                                    map[i - 1][j].depth = stepDepth;
                                    map[i - 1][j].type = PointType.waypoint;
                                }
                                else if ((map[i][j].type == PointType.waypoint || map[i][j].type == PointType.start) && map[i - 1][j].type == PointType.end)
                                {
                                    targetReached = true;
                                    map[i - 1][j].depth = stepDepth;
                                    break;
                                }
                            }
                        }
                        if (targetReached)
                        {
                            break;
                        }
                    }
                    stepDepth++;
                }
                x = ex;
                y = ey;
                stepDepth = map[x][y].depth;
            }

            private void FixPoint(int x, int y, point LastTraced)
            {
                map[x][y].setTarget(LastTraced);
                LastTraced.setOwner(map[x][y]);
                if (map[x][y].type != PointType.start)
                {
                    map[x][y].type = PointType.fixed_point;
                }
            }

            public Map(point[][] map, int size)
            {
                this.map = map;
                this.size = size;
                Utilities.Utilities.debug_x_pos = Console.CursorLeft;
                Utilities.Utilities.debug_y_pos = Console.CursorTop;
                Utilities.Utilities.last_debug_x = Console.CursorLeft;
                Utilities.Utilities.last_debug_y = Console.CursorTop + size;
                for (int i = map.Length - 1; i >= 0; i--)
                {
                    for (int j = map[i].Length - 1; j >= 0; j--)
                    {
                        if ((map[i][j].meta is Jmp || map[i][j].meta is Jnc) && map[i][j].isUserPlaced && !map[i][j].isAnalyzed)
                        {
                            Trace(i, j, GetLinkedPoint(map[i][j]).row, GetLinkedPoint(map[i][j]).column);
                        }
                    }
                }
                while (!CheckTracks())
                {
                    Utilities.Utilities.VerbouseOut("BUG_FIXER", "Checking...", ConsoleColor.Yellow);
                }
                Utilities.Utilities.VerbouseOut("BUG_FIXER", "All errors fixed", ConsoleColor.Green);
                //All bugs were fixed, now we can bake the map (fix all jumps and bridges)
                BakeMap();
                Utilities.Utilities.VerbouseOut("TRACER", "Tracing and baking done", ConsoleColor.Green);
                PrintAllTracks();
            }

            private void PrintAllTracks()
            {
                Utilities.Utilities.VerbouseOut("TRACER", "TRACKS LISTING", ConsoleColor.Cyan);
                for (int i = 0; i<size; i++)
                {
                    for (int j = 0; j<size; j++)
                    {
                        if (map[i][j].type == PointType.boundary_point && map[i][j].owner == null)
                        {
                            point pointer = map[i][j].target;
                            Utilities.Utilities.VerbouseOut("TRACER", "Initial point: "+i.ToString()+":"+j.ToString(), ConsoleColor.Cyan);
                            while (pointer.target != null)
                            {
                                Utilities.Utilities.VerbouseOut("Helper point: " + pointer.row.ToString() + ":" + pointer.column.ToString(), ConsoleColor.Blue);
                                pointer = pointer.target;
                            }
                            Utilities.Utilities.VerbouseOut("TRACER", "End point: " + pointer.row.ToString() + ":" + pointer.column.ToString(), ConsoleColor.DarkMagenta);
                        }
                    }
                }
            }

            private void BakeMap()
            {
                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        point bakingPoint = map[i][j];
                        if (bakingPoint.type == PointType.bridge)
                        {
                            int empty_point = j + 1;
                            while (map[i][empty_point].type != PointType.empty)
                            {
                                empty_point++;
                            }
                            Utilities.Utilities.VerbouseOut("BAKER", "Linking bridge to " + empty_point.ToString());
                            bakingPoint.meta = new JmpI(empty_point);
                        }
                        else if (bakingPoint.type == PointType.old_fixed_point)
                        {
                            if (bakingPoint.target.row == bakingPoint.row)
                                bakingPoint.meta = new JmpI(bakingPoint.target.column);
                            else if (bakingPoint.target.column == bakingPoint.column)
                                bakingPoint.meta = new Swi(bakingPoint.target.row);
                            Utilities.Utilities.VerbouseOut("BAKER", "Making helping point");
                        }
                        else if (bakingPoint.type == PointType.boundary_point && bakingPoint.owner == null)
                        {
                            if (bakingPoint.meta is Jmp)
                            {
                                bakingPoint.meta = new JmpI(bakingPoint.target.column);
                            }
                            else if (bakingPoint.meta is Jnc)
                            {
                                if (bakingPoint.target.target == null)
                                {
                                    bakingPoint.meta = new JncI(bakingPoint.target.column);
                                    Utilities.Utilities.VerbouseOut("BAKER", "Making strict JNC");
                                }
                                else
                                {
                                    bakingPoint.meta = new JncI(bakingPoint.target.column);
                                    Utilities.Utilities.VerbouseOut("BAKER", "Making JNC to crosslayer bridge");
                                }
                            }
                            else if (bakingPoint.meta is Swi)
                            {
                                bakingPoint.meta = new Swi(bakingPoint.target.row);
                                Utilities.Utilities.VerbouseOut("BAKER", "Baking SWI");
                            }
                        }
                    }
                }
            }

            private point GetLinkedPoint(point point)
            {
                FastAdd target = point.meta.FastAdd;
                int targetAddr = target.toInt();
                int targetI = targetAddr / (Program.eightBit ? 256 : 16);
                int targetJ = targetAddr - (targetI * ((Program.eightBit ? 256 : 16) - 1));
                point a = new point(PointType.end, targetI, targetJ, 0, map[targetI][targetJ].meta, false, false);
                return a;
            }

            private void DebugMap()
            {
                if (Program.verboseMode)
                {
                    PrintMap();
                    System.Console.WriteLine();
                }
            }

            private void PrintMap()
            {
                System.Console.SetCursorPosition(Utilities.Utilities.debug_x_pos, Utilities.Utilities.debug_y_pos);
                System.Console.Write(" /");
                for (int i = -1; i < map.Length; i++)
                {
                    if (i != -1)
                    {
                        if (i < 10)
                        {
                            System.Console.Write(" " + i.ToString());
                        }
                        else
                        {
                            System.Console.Write(i);
                        }
                    }
                    for (int j = 0; j < map.Length; j++)
                    {
                        if (i == -1)
                        {
                            if (j < 10)
                            {
                                System.Console.Write(" " + j.ToString());
                            }
                            else
                            {
                                System.Console.Write(j);
                            }
                        }
                        else
                        {
                            if (map[i][j].meta is Swi && j == 15 && map[i][j].meta.FastAdd.toInt() == i + 1)
                            {
                                System.Console.BackgroundColor = System.ConsoleColor.DarkMagenta;
                                System.Console.Write(">>");
                                System.Console.ResetColor();
                            }
                            switch (map[i][j].type)
                            {
                                case PointType.empty:
                                    System.Console.Write("  ");
                                    break;
                                case PointType.empty_nop:
                                    System.Console.BackgroundColor = System.ConsoleColor.Green;
                                    System.Console.Write("  ");
                                    System.Console.ResetColor();
                                    break;
                                case PointType.start:
                                    System.Console.ForegroundColor = System.ConsoleColor.Green;
                                    System.Console.Write(" S");
                                    System.Console.ResetColor();
                                    break;
                                case PointType.end:
                                    System.Console.ForegroundColor = System.ConsoleColor.Red;
                                    System.Console.Write(" E");
                                    System.Console.ResetColor();
                                    break;
                                case PointType.fixed_point:
                                    System.Console.Write(" o");
                                    break;
                                case PointType.waypoint:
                                    System.Console.ForegroundColor = System.ConsoleColor.Yellow;
                                    if (map[i][j].depth < 10)
                                    {
                                        System.Console.Write(" " + map[i][j].depth);
                                    }
                                    else
                                    {
                                        System.Console.Write(map[i][j].depth);
                                    }
                                    System.Console.ResetColor();
                                    break;
                                case PointType.switchPage:
                                    System.Console.ForegroundColor = System.ConsoleColor.Cyan;
                                    System.Console.Write(" S");
                                    System.Console.ResetColor();
                                    break;
                                case PointType.bridge:
                                    System.Console.ForegroundColor = System.ConsoleColor.Magenta;
                                    System.Console.Write(" B");
                                    System.Console.ResetColor();
                                    break;
                                case PointType.old_fixed_point:
                                    System.Console.ForegroundColor = System.ConsoleColor.DarkYellow;
                                    System.Console.Write(" o");
                                    System.Console.ResetColor();
                                    break;
                                case PointType.boundary_point:
                                    System.Console.ForegroundColor = System.ConsoleColor.DarkGreen;
                                    System.Console.Write(" o");
                                    System.Console.ResetColor();
                                    break;
                            }
                        }
                    }
                    System.Console.WriteLine();
                }
                Console.SetCursorPosition(Utilities.Utilities.last_debug_x, Utilities.Utilities.last_debug_y);
            }
        }
    }
}