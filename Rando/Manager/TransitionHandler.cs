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
            // Must happen after ApplyTransitionSettings in base rando
            // but before SelectTransitions in TrandoPlus so that it also
            // works with door rando.
            RequestBuilder.OnUpdate.Subscribe(-900f, AddRandomizableTransitions);
            // But if we are not randomizing our transitions, this should happen after
            // SelectTransitions, so that door rando doesn't see those transitions
            // and randomizes them.
            // We still need to register the transitions because logic depends upon
            // them.
            RequestBuilder.OnUpdate.Subscribe(-700f, AddVanillaTransitions);
        }

        private static void AddRandomizableTransitions(RequestBuilder rb)
        {
            if (!(GolfManager.GlobalSettings.Enabled && GolfManager.GlobalSettings.CourseTransitions))
            {
                return;
            }

            List<DoorDef> newDoors = GetDoorDefs(rb);

            foreach (DoorDef d in newDoors)
            {
                CreateTransition(rb, d.Door);
                CreateTransition(rb, d.OtherSide);
            }

            // actually put the transitions in the pool
            if (rb.gs.TransitionSettings.Mode != TransitionSettings.TransitionMode.RoomRandomizer)
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

        private static void AddVanillaTransitions(RequestBuilder rb)
        {
            if (!GolfManager.GlobalSettings.Enabled || GolfManager.GlobalSettings.CourseTransitions)
            {
                return;
            }

            List<DoorDef> newDoors = GetDoorDefs(rb);

            foreach (DoorDef d in newDoors)
            {
                CreateTransition(rb, d.Door);
                CreateTransition(rb, d.OtherSide);
            }

            foreach (DoorDef d in newDoors)
            {
                rb.EnsureVanillaSourceTransition(d.Door.Name);
                rb.EnsureVanillaSourceTransition(d.OtherSide.Name);
            }
        }

        private record DoorDef(TransitionDef Door, TransitionDef OtherSide) {}

        private static List<DoorDef> GetDoorDefs(RequestBuilder rb)
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
                TransitionDirection otherSideDir = GetOrigTransitionDirection(rb, scene, gs.startTransition);
                // Door rando expects all door transitions to go sideways
                // (since all vanilla ones do). We work around that by lying about
                // doors that go downwards.
                // Regular transition rando doesn't care.
                if (otherSideDir == TransitionDirection.Top)
                {
                    otherSideDir = TransitionDirection.Right;
                }
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
                        Direction = otherSideDir,
                        IsTitledAreaTransition = false,
                        IsMapAreaTransition = false,
                        Sides = TransitionSides.Both,
                    }
                ));
            }

            return newDoors;
        }

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