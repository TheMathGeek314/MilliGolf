using ItemChanger;
using System;
using System.Collections.Generic;
using System.Linq;
using RandomizerMod.RC;
using RandomizerMod.RandomizerData;
using RandomizerMod.Settings;

namespace MilliGolf.Rando.Manager
{
    internal static class TransitionHandler
    {
        internal static void Hook()
        {
            // must happen after ApplyTransitionSettings but before
            // PlaceUnrandomizedTransitions
            RequestBuilder.OnUpdate.Subscribe(-900f, AddTransitions);
        }

        private static void AddTransitions(RequestBuilder rb)
        {
            List<DoorDef> newDoors = new();

            newDoors.Add(new(
                new()
                {
                    SceneName = SceneNames.Town,
                    DoorName = MilliGolf.golfTentTransition,
                    VanillaTarget = $"{SceneNames.GG_Workshop}[left1{MilliGolf.golfTransitionSuffix}]",
                    Direction = TransitionDirection.Door,
                    IsTitledAreaTransition = false,
                    IsMapAreaTransition = false,
                    Sides = TransitionSides.Both,
                },
                new()
                {
                    SceneName = SceneNames.GG_Workshop,
                    DoorName = "left1" + MilliGolf.golfTransitionSuffix,
                    VanillaTarget = $"{SceneNames.Town}[{MilliGolf.golfTentTransition}]",
                    Direction = TransitionDirection.Left,
                    IsTitledAreaTransition = false,
                    IsMapAreaTransition = false,
                    Sides = TransitionSides.Both,
                }
            ));
            for (int i = 0; i < golfScene.courseList.Count; i++)
            {
                string scene = golfScene.courseList[i];
                golfScene gs = golfScene.courseDict[scene];
                //TransitionDirection otherSideDir = GetOrigTransitionDirection(rb, scene, gs.startTransition); 
                newDoors.Add(new(
                    new()
                    {
                        SceneName = SceneNames.GG_Workshop,
                        DoorName = $"door{i+1}{MilliGolf.golfTransitionSuffix}",
                        VanillaTarget = $"{scene}[{gs.startTransition}{MilliGolf.golfTransitionSuffix}]",
                        Direction = TransitionDirection.Door,
                        IsTitledAreaTransition = false,
                        IsMapAreaTransition = false,
                        Sides = TransitionSides.Both,
                    },
                    new()
                    {
                        SceneName = scene,
                        DoorName = gs.startTransition + MilliGolf.golfTransitionSuffix,
                        VanillaTarget = $"{SceneNames.GG_Workshop}[door{i+1}{MilliGolf.golfTransitionSuffix}]",
                        //Direction = GetOrigTransitionDirection(rb, scene, gs.startTransition),
                        Direction = TransitionDirection.Left,
                        IsTitledAreaTransition = false,
                        IsMapAreaTransition = false,
                        Sides = TransitionSides.Both,
                    }
                ));
            }

            foreach (DoorDef d in newDoors)
            {
                CreateTransition(rb, d.Door);
                CreateTransition(rb, d.OtherSide);
            }

            // actually put the transitions in the pool
            if (rb.gs.TransitionSettings.Mode == TransitionSettings.TransitionMode.None)
            {
                foreach (DoorDef d in newDoors)
                {
                    rb.EnsureVanillaSourceTransition(d.Door.Name);
                    rb.EnsureVanillaSourceTransition(d.OtherSide.Name);
                }
                return;
            }

            switch (rb.gs.TransitionSettings.TransitionMatching)
            {
                case TransitionSettings.TransitionMatchingSetting.MatchingDirections:
                case TransitionSettings.TransitionMatchingSetting.MatchingDirectionsAndNoDoorToDoor:
                    List<SymmetricTransitionGroupBuilder> symBuilders = rb.EnumerateTransitionGroups().OfType<SymmetricTransitionGroupBuilder>().ToList();
                    SymmetricTransitionGroupBuilder 
                        horizontal = symBuilders.First(b => b.label == RBConsts.InLeftOutRightGroup),
                        vertical = symBuilders.First(b => b.label == RBConsts.InTopOutBotGroup);

                    // Randomize which doors will have to be paired with a
                    // right, left, or up transition.
                    for (int i = 0; i < newDoors.Count; i++)
                    {
                        int j = rb.rng.Next(newDoors.Count - i) + i;
                        DoorDef m = new(newDoors[j].Door, newDoors[i].OtherSide);
                        newDoors[j] = new(newDoors[i].Door, newDoors[j].OtherSide);
                        newDoors[i] = m;
                    }

                    foreach (DoorDef d in newDoors)
                    {
                        switch (d.OtherSide.Direction)
                        {
                            case TransitionDirection.Left:
                                horizontal.Group1.Add(d.Door.Name);
                                horizontal.Group2.Add(d.OtherSide.Name);
                                break;
                            case TransitionDirection.Right:
                                horizontal.Group1.Add(d.OtherSide.Name);
                                horizontal.Group2.Add(d.Door.Name);
                                break;
                            case TransitionDirection.Top:
                                vertical.Group1.Add(d.Door.Name);
                                vertical.Group2.Add(d.OtherSide.Name);
                                break;
                            default:
                                throw new InvalidOperationException($"unexpected transition direction: {d.OtherSide.Direction}");
                        }
                    }
                    break;
                case TransitionSettings.TransitionMatchingSetting.NonmatchingDirections:
                    SelfDualTransitionGroupBuilder asymBuilder = rb.EnumerateTransitionGroups().OfType<SelfDualTransitionGroupBuilder>().First(b => b.label == RBConsts.TwoWayGroup);
                    foreach (DoorDef d in newDoors)
                    {
                        asymBuilder.Transitions.Add(d.Door.Name);
                        asymBuilder.Transitions.Add(d.OtherSide.Name);
                    }
                    break;
            }
        }

        private record DoorDef(TransitionDef Door, TransitionDef OtherSide) {}

        private static TransitionDirection GetOrigTransitionDirection(RequestBuilder rb, string scene, string transition)
        {
            string fullName = $"{scene}[{transition}]";
            if (!rb.TryGetTransitionDef(fullName, out TransitionDef orig))
            {
                throw new InvalidOperationException($"missing transition: {fullName}");
            }
            return orig.Direction;
        }

        private static void CreateTransition(RequestBuilder rb, TransitionDef tdef)
        {
            rb.EditTransitionRequest(tdef.Name, info =>
            {
                info.getTransitionDef = () => tdef;
            });
        }
    }
}