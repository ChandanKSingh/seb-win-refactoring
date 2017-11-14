﻿/*
 * Copyright (c) 2017 ETH Zürich, Educational Development and Technology (LET)
 * 
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SafeExamBrowser.Contracts.I18n;
using SafeExamBrowser.Contracts.Logging;
using SafeExamBrowser.Contracts.UserInterface;
using SafeExamBrowser.Core.Notifications;

namespace SafeExamBrowser.Core.UnitTests.Notifications
{
	[TestClass]
	public class LogNotificationControllerTests
	{
		private Mock<ILogger> loggerMock;
		private Mock<IText> textMock;
		private Mock<IUserInterfaceFactory> uiFactoryMock;

		[TestInitialize]
		public void Initialize()
		{
			loggerMock = new Mock<ILogger>();
			textMock = new Mock<IText>();
			uiFactoryMock = new Mock<IUserInterfaceFactory>();
		}

		[TestMethod]
		public void MustCloseWindowWhenTerminating()
		{
			var button = new NotificationButtonMock();
			var window = new Mock<IWindow>();
			var sut = new LogNotificationController(loggerMock.Object, textMock.Object, uiFactoryMock.Object);

			uiFactoryMock.Setup(u => u.CreateLogWindow(It.IsAny<ILogger>(), It.IsAny<IText>())).Returns(window.Object);
			sut.RegisterNotification(button);
			button.Click();
			sut.Terminate();

			window.Verify(w => w.Close());
		}

		[TestMethod]
		public void MustOpenOnlyOneWindow()
		{
			var button = new NotificationButtonMock();
			var window = new Mock<IWindow>();
			var sut = new LogNotificationController(loggerMock.Object, textMock.Object, uiFactoryMock.Object);

			uiFactoryMock.Setup(u => u.CreateLogWindow(It.IsAny<ILogger>(), It.IsAny<IText>())).Returns(window.Object);
			sut.RegisterNotification(button);
			button.Click();
			button.Click();
			button.Click();
			button.Click();
			button.Click();

			uiFactoryMock.Verify(u => u.CreateLogWindow(It.IsAny<ILogger>(), It.IsAny<IText>()), Times.Once);
			window.Verify(u => u.Show(), Times.Once);
			window.Verify(u => u.BringToForeground(), Times.Exactly(4));
		}

		[TestMethod]
		public void MustSubscribeToClickEvent()
		{
			var button = new NotificationButtonMock();
			var sut = new LogNotificationController(loggerMock.Object, textMock.Object, uiFactoryMock.Object);

			sut.RegisterNotification(button);

			Assert.IsTrue(button.HasSubscribed);
		}
	}
}