using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zutatensuppe.D2Reader;
using Zutatensuppe.D2Reader.Models;
using Zutatensuppe.D2Reader.Struct.Item;
using Zutatensuppe.DiabloInterface.Business.AutoSplits;
using Zutatensuppe.DiabloInterface.Business.Services;
using Zutatensuppe.DiabloInterface.Business.Settings;

namespace DiabloInterface.Test.Business.Services
{
    [TestClass]
    public class AutoSplitServiceTest
    {
        [TestMethod]
        public void ShouldNotSplitIfDisabled()
        {
            var split = new AutoSplit(
                "one",
                AutoSplit.SplitType.CharLevel,
                10,
                (short)GameDifficulty.Normal
            );
            var appSettings = new ApplicationSettings();
            appSettings.DoAutosplit = false;
            appSettings.Autosplits.Add(split);
            var settingsServiceMock = new Mock<ISettingsService>();
            settingsServiceMock.SetupGet(x => x.CurrentSettings).Returns(appSettings);
            var gameService = new GameServiceMock();
            AutoSplitService autoSplitService = new AutoSplitService(
                settingsServiceMock.Object,
                gameService,
                new Mock<KeyService>().Object
            );
            var quests = new List<Quest>();
            var questCollection = new List<QuestCollection>();
            questCollection.Add(new QuestCollection(quests)); // NORMAL
            questCollection.Add(new QuestCollection(quests)); // NIGHTMARE
            questCollection.Add(new QuestCollection(quests)); // HELL


            var mock = new Mock<Character>();
            mock.SetupGet(x => x.Level).Returns(9);

            // test autosplit by level
            gameService.triggerDataRead(null, new DataReadEventArgs(
                mock.Object,
                new Dictionary<BodyLocation, string>(), // item strings
                0, // area
                GameDifficulty.Normal, // difficulty
                1, // players x
                new List<int>(), // item ids
                true, // is autosplit char
                questCollection,
                0 // game counter
            ));
            Assert.AreEqual(false, split.IsReached);

            mock.SetupGet(x => x.Level).Returns(10);
            gameService.triggerDataRead(null, new DataReadEventArgs(
                mock.Object,
                new Dictionary<BodyLocation, string>(), // item strings
                0, // area
                GameDifficulty.Normal, // difficulty
                1, // players x
                new List<int>(), // item ids
                true, // is autosplit char
                questCollection,
                0 // game counter
            ));
            Assert.AreEqual(false, split.IsReached);
        }

        [TestMethod]
        public void ShouldReachAutosplits()
        {
            var splitOnGameStart = new AutoSplit(
                "game start",
                AutoSplit.SplitType.Special,
                1,
                (short)GameDifficulty.Normal
            );
            var splitOnGameClear = new AutoSplit(
                "100% done",
                AutoSplit.SplitType.Special,
                2,
                (short)GameDifficulty.Normal
            );
            var splitOnGameClearAllDifficulties = new AutoSplit(
                "100% done all difficulties",
                AutoSplit.SplitType.Special,
                3,
                (short)GameDifficulty.Normal
            );
            var splitOnCharLevel10 = new AutoSplit(
                "char level 10",
                AutoSplit.SplitType.CharLevel,
                10,
                (short)GameDifficulty.Normal
            );
            var splitOnArea1 = new AutoSplit(
                "area 1",
                AutoSplit.SplitType.Area,
                1,
                (short)GameDifficulty.Normal
            );
            var splitOnItem1 = new AutoSplit(
                "item 1",
                AutoSplit.SplitType.Item,
                1,
                (short)GameDifficulty.Normal
            );
            var splitOnGem1 = new AutoSplit(
                "gem 1",
                AutoSplit.SplitType.Gems,
                1,
                (short)GameDifficulty.Normal
            );
            var splitOnQuest81 = new AutoSplit(
                "quest 81 (den of evil)",
                AutoSplit.SplitType.Quest,
                81,
                (short)GameDifficulty.Normal
            );
            var appSettings = new ApplicationSettings();
            appSettings.DoAutosplit = true;
            appSettings.Autosplits.Add(splitOnGameStart);
            appSettings.Autosplits.Add(splitOnGameClear);
            appSettings.Autosplits.Add(splitOnGameClearAllDifficulties);
            appSettings.Autosplits.Add(splitOnCharLevel10);
            appSettings.Autosplits.Add(splitOnArea1);
            appSettings.Autosplits.Add(splitOnItem1);
            appSettings.Autosplits.Add(splitOnGem1);
            appSettings.Autosplits.Add(splitOnQuest81);
            var settingsServiceMock = new Mock<ISettingsService>();
            settingsServiceMock.SetupGet(x => x.CurrentSettings).Returns(appSettings);
            var gameService = new GameServiceMock();
            AutoSplitService autoSplitService = new AutoSplitService(
                settingsServiceMock.Object,
                gameService,
                new Mock<KeyService>().Object
            );

            var normalQuests = new List<Quest>() {
                QuestFactory.Create(QuestId.DenOfEvil, 0),
                QuestFactory.Create(QuestId.Andariel, 0),
            };

            var nightmareQuests = new List<Quest>() {
                QuestFactory.Create(QuestId.DenOfEvil, 0),
                QuestFactory.Create(QuestId.Andariel, 0),
            };

            var hellQuests = new List<Quest>() {
                QuestFactory.Create(QuestId.DenOfEvil, 0),
                QuestFactory.Create(QuestId.Andariel, 0),
            };

            var questCollection = new List<QuestCollection>();
            questCollection.Add(new QuestCollection(normalQuests));
            questCollection.Add(new QuestCollection(nightmareQuests));
            questCollection.Add(new QuestCollection(hellQuests));

            var characterMock = new Mock<Character>();
            characterMock.SetupGet(x => x.Level).Returns(9);

            var itemStrings = new Dictionary<BodyLocation, string>();
            var itemsIds = new List<int>();

            // test autosplit by game start
            Assert.AreEqual(false, splitOnGameStart.IsReached);
            gameService.triggerDataRead(null, new DataReadEventArgs(
                characterMock.Object,
                itemStrings,
                0, // area
                GameDifficulty.Normal, // difficulty
                1, // players x
                itemsIds,
                true, // is autosplit char
                questCollection,
                0 // game counter
            ));
            Assert.AreEqual(true, splitOnGameStart.IsReached);

            // test autosplit by level
            Assert.AreEqual(false, splitOnCharLevel10.IsReached);
            gameService.triggerDataRead(null, new DataReadEventArgs(
                characterMock.Object,
                itemStrings,
                0, // area
                GameDifficulty.Normal, // difficulty
                1, // players x
                itemsIds,
                true, // is autosplit char
                questCollection,
                0 // game counter
            ));
            Assert.AreEqual(false, splitOnCharLevel10.IsReached);

            characterMock.SetupGet(x => x.Level).Returns(10);
            gameService.triggerDataRead(null, new DataReadEventArgs(
                characterMock.Object,
                itemStrings,
                0, // area
                GameDifficulty.Normal, // difficulty
                1, // players x
                itemsIds,
                true, // is autosplit char
                questCollection,
                0 // game counter
            ));
            Assert.AreEqual(true, splitOnCharLevel10.IsReached);

            // test autosplit by area
            Assert.AreEqual(false, splitOnArea1.IsReached);
            gameService.triggerDataRead(null, new DataReadEventArgs(
                characterMock.Object,
                itemStrings,
                1, // area
                GameDifficulty.Normal, // difficulty
                1, // players x
                itemsIds,
                true, // is autosplit char
                questCollection,
                0 // game counter
            ));
            Assert.AreEqual(true, splitOnArea1.IsReached);

            // test autosplit by item
            Assert.AreEqual(false, splitOnItem1.IsReached);
            Assert.AreEqual(false, splitOnGem1.IsReached);
            itemsIds.Add(1);
            gameService.triggerDataRead(null, new DataReadEventArgs(
                characterMock.Object,
                itemStrings,
                0, // area
                GameDifficulty.Normal, // difficulty
                1, // players x
                itemsIds,
                true, // is autosplit char
                questCollection,
                0 // game counter
            ));
            Assert.AreEqual(true, splitOnItem1.IsReached);
            Assert.AreEqual(true, splitOnGem1.IsReached);

            // test autosplit by quest
            Assert.AreEqual(false, splitOnQuest81.IsReached);
            normalQuests.Clear();
            normalQuests.Add(QuestFactory.Create(QuestId.DenOfEvil, 1 << 0));
            normalQuests.Add(QuestFactory.Create(QuestId.Andariel, 0));
            gameService.triggerDataRead(null, new DataReadEventArgs(
                characterMock.Object,
                itemStrings,
                0, // area
                GameDifficulty.Normal, // difficulty
                1, // players x
                itemsIds,
                true, // is autosplit char
                questCollection,
                0 // game counter
            ));
            Assert.AreEqual(true, splitOnQuest81.IsReached);

            // test autosplit on game clear
            Assert.AreEqual(false, splitOnGameClear.IsReached);
            normalQuests.Clear();
            normalQuests.Add(QuestFactory.Create(QuestId.DenOfEvil, 1 << 0));
            normalQuests.Add(QuestFactory.Create(QuestId.Andariel, 1 << 0));
            gameService.triggerDataRead(null, new DataReadEventArgs(
                characterMock.Object,
                itemStrings,
                0, // area
                GameDifficulty.Normal, // difficulty
                1, // players x
                itemsIds,
                true, // is autosplit char
                questCollection,
                0 // game counter
            ));
            Assert.AreEqual(true, splitOnGameClear.IsReached);

            // test autosplit on game clear all difficulties
            Assert.AreEqual(false, splitOnGameClearAllDifficulties.IsReached);
            nightmareQuests.Clear();
            nightmareQuests.Add(QuestFactory.Create(QuestId.DenOfEvil, 1 << 0));
            nightmareQuests.Add(QuestFactory.Create(QuestId.Andariel, 1 << 0));
            gameService.triggerDataRead(null, new DataReadEventArgs(
                characterMock.Object,
                itemStrings,
                0, // area
                GameDifficulty.Normal, // difficulty
                1, // players x
                itemsIds,
                true, // is autosplit char
                questCollection,
                0 // game counter
            ));
            Assert.AreEqual(false, splitOnGameClearAllDifficulties.IsReached);
            hellQuests.Clear();
            hellQuests.Add(QuestFactory.Create(QuestId.DenOfEvil, 1 << 0));
            hellQuests.Add(QuestFactory.Create(QuestId.Andariel, 1 << 0));
            gameService.triggerDataRead(null, new DataReadEventArgs(
                characterMock.Object,
                itemStrings,
                0, // area
                GameDifficulty.Normal, // difficulty
                1, // players x
                itemsIds,
                true, // is autosplit char
                questCollection,
                0 // game counter
            ));
            Assert.AreEqual(true, splitOnGameClearAllDifficulties.IsReached);
        }

        private Character CreateCharacter(int lvl)
        {
            var mock = new Mock<Character>();
            mock.SetupGet(x => x.Level).Returns(lvl);
            return mock.Object;
        }
    }

    class GameServiceMock : IGameService
    {
        public GameDifficulty TargetDifficulty { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public D2DataReader DataReader => throw new NotImplementedException();

        public event EventHandler<CharacterCreatedEventArgs> CharacterCreated;
        public event EventHandler<DataReadEventArgs> DataRead;

        public void triggerDataRead(object sender, DataReadEventArgs e)
        {
            DataRead?.Invoke(sender, e);
        }
    }
}