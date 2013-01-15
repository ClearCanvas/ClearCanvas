#region License

// Copyright (c) 2013, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This file is part of the ClearCanvas RIS/PACS open source project.
//
// The ClearCanvas RIS/PACS open source project is free software: you can
// redistribute it and/or modify it under the terms of the GNU General Public
// License as published by the Free Software Foundation, either version 3 of the
// License, or (at your option) any later version.
//
// The ClearCanvas RIS/PACS open source project is distributed in the hope that it
// will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General
// Public License for more details.
//
// You should have received a copy of the GNU General Public License along with
// the ClearCanvas RIS/PACS open source project.  If not, see
// <http://www.gnu.org/licenses/>.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Desktop.Validation;
using System.Collections;
using ClearCanvas.Ris.Application.Extended.Common.OrderNotes;

namespace ClearCanvas.Ris.Client.Workflow.Extended
{
	/// <summary>
	/// Extension point for views onto <see cref="OrderNoteConversationComponent"/>.
	/// </summary>
	[ExtensionPoint]
	public sealed class OrderNoteConversationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// PreliminaryDiagnosisConversationComponent class.
	/// </summary>
	[AssociateView(typeof(OrderNoteConversationComponentViewExtensionPoint))]
	public partial class OrderNoteConversationComponent : ApplicationComponent
	{
		#region StaffAndGroupLookupHandler class

		class StaffAndGroupLookupHandler : LookupHandlerAggregator
		{
			private readonly DesktopWindow _desktopWindow;

			internal StaffAndGroupLookupHandler(DesktopWindow desktopWindow)
				: base(new Dictionary<ILookupHandler, Type>
							{
								{new StaffLookupHandler(desktopWindow), typeof (StaffSummary)},
								{new StaffGroupLookupHandler(desktopWindow, false), typeof (StaffGroupSummary)}
							})
			{
				_desktopWindow = desktopWindow;
			}

			protected override bool ResolveNameInteractive(string query, out object result)
			{
				result = null;
				var component = new StaffOrStaffGroupSummaryComponent();

				var exitCode = LaunchAsDialog(_desktopWindow, component, SR.TitleStaffOrStaffGroups);

				if (exitCode == ApplicationComponentExitCode.Accepted)
				{
					if (component.IsSelectingStaff)
						result = component.SelectedStaff;
					else
						result = component.SelectedStaffGroup;
				}

				return (result != null);
			}

			public override string FormatItem(object item)
			{
				var s = base.FormatItem(item);

				// if the staff group summary is formatted properly, add a (Staff Group) to help
				// identify it as a group, rather than a staff
				if (!string.IsNullOrEmpty(s) && item is StaffGroupSummary)
					s += " (Staff Group)";

				return s;
			}
		}

		#endregion

		#region Private Fields

		private EntityRef _orderRef;
		private List<OrderNoteDetail> _orderNotes;

		private readonly List<TemplateData> _templateChoices;
		private TemplateData _selectedTemplate;

		private readonly List<SoftKeyData> _defaultSoftKeys;
		private readonly List<SoftKeyData> _softKeys;

		private readonly IList<string> _orderNoteCategories;

		private string _body;

		private bool _urgent;

		private IList<StaffGroupSummary> _onBehalfOfChoices;
		private StaffGroupSummary _onBehalfOf;

		private RecipientTable _recipients;
		private CrudActionModel _recipientsActionModel;
		private Checkable<RecipientTableItem> _selectedRecipient;

		private ICannedTextLookupHandler _cannedTextLookupHandler;

		private OrderNoteViewComponent _orderNoteViewComponent;
		private ChildComponentHost _orderNotesComponentHost;

		private readonly StaffGroupSummary _emptyStaffGroup = new StaffGroupSummary();

		private event EventHandler _newRecipientAdded;

		#endregion

		#region Constructors

		public OrderNoteConversationComponent(EntityRef orderRef, string orderNoteCategory, string templatesXml, string softKeysXml)
			: this(orderRef, new[] { orderNoteCategory }, templatesXml, softKeysXml)
		{
		}

		public OrderNoteConversationComponent(EntityRef orderRef, string[] orderNoteCategories, string templatesXml, string softKeysXml)
		{
			Platform.CheckForNullReference(orderRef, "orderRef");
			Platform.CheckForNullReference(orderNoteCategories, "orderNoteCategories");

			_orderRef = orderRef;
			_softKeys = new List<SoftKeyData>();
			_orderNoteCategories = orderNoteCategories;
			_templateChoices = LoadTemplates(templatesXml);
			_defaultSoftKeys = LoadSoftKeys(softKeysXml);

			this.Validation.Add(new ValidationRule("SelectedRecipient",
				delegate
				{
					// if body is non-empty (a new note is being posted), must have at least 1 recip
					// unless template explicitly specified that notes can be posted without recipients
					var atLeastOneRecipient = CollectionUtils.Contains(_recipients.Items, r => r.IsChecked);
					var allowPostWithoutRecips = _selectedTemplate != null && _selectedTemplate.AllowPostWithoutRecipients;
					return new ValidationResult(allowPostWithoutRecips || atLeastOneRecipient || IsBodyEmpty, SR.MessageNoRecipientsSelected);
				}));

			this.Validation.Add(new ValidationRule("Body",
				delegate
				{
					var isNotEmptyNewNote = !(this.IsBodyEmpty && !_orderNoteViewComponent.HasAcknowledgeableNotes);
					return new ValidationResult(isNotEmptyNewNote, SR.MessageBodyCannotBeEmpty);
				}));

			this.Validation.Add(new ValidationRule("SelectedTemplate",
				delegate
				{
					var nonNullWhenChoicesExist = !(_selectedTemplate == null && this.TemplateChoicesVisible);
					return new ValidationResult(nonNullWhenChoicesExist, SR.MessageMustSelectTemplate);
				}));
		}

		#endregion

		#region ApplicationComponent overrides

		/// <summary>
		/// Called by the host to initialize the application component.
		/// </summary>
		public override void Start()
		{
			// init lookup handlers
			_cannedTextLookupHandler = new CannedTextLookupHandler(this.Host.DesktopWindow);

			// init recip table here, and not in constructor, because it relies on Host being set
			_recipients = new RecipientTable(this);

			// if exactly 1 template choice, then it is selected
			_selectedTemplate = _templateChoices.Count == 1 ? _templateChoices[0] : null;

			// create soft keys
			UpdateSoftKeys();

			// load the existing conversation, plus editor form data
			GetConversationEditorFormDataResponse formDataResponse = null;
			Platform.GetService<IOrderNoteService>(
				service =>
				{
					var formDataRequest = new GetConversationEditorFormDataRequest(
						_selectedTemplate != null ? _selectedTemplate.GetStaffRecipients() : new List<string>(),
						_selectedTemplate != null ? _selectedTemplate.GetGroupRecipients() : new List<string>());
					formDataResponse = service.GetConversationEditorFormData(formDataRequest);

					var request = new GetConversationRequest(_orderRef, new List<string>(_orderNoteCategories), false);
					var response = service.GetConversation(request);

					_orderRef = response.OrderRef;
					_orderNotes = response.OrderNotes;
				});


			// init on-behalf of choices
			_onBehalfOfChoices = formDataResponse.OnBehalfOfGroupChoices;
			_onBehalfOfChoices.Insert(0, _emptyStaffGroup);

			// initialize from template (which may be null)
			InitializeFromTemplate(_selectedTemplate, formDataResponse.RecipientStaffs, formDataResponse.RecipientStaffGroups);

			// build the action model
			_recipientsActionModel = new CrudActionModel(true, false, true, new ResourceResolver(this.GetType(), true));
			_recipientsActionModel.Add.SetClickHandler(AddRecipient);
			_recipientsActionModel.Delete.SetClickHandler(DeleteRecipient);

			// init conversation view component
			_orderNoteViewComponent = new OrderNoteViewComponent(_orderNotes);
			_orderNoteViewComponent.CheckedItemsChanged += delegate { NotifyPropertyChanged("CompleteButtonLabel"); };
			_orderNotesComponentHost = new ChildComponentHost(this.Host, _orderNoteViewComponent);
			_orderNotesComponentHost.StartComponent();

			base.Start();
		}

		public override void Stop()
		{
			if (_orderNotesComponentHost != null)
			{
				_orderNotesComponentHost.StopComponent();
				_orderNotesComponentHost = null;
			}

			base.Stop();
		}

		#endregion

		#region Presentation Model

		public ApplicationComponentHost OrderNotesHost
		{
			get { return _orderNotesComponentHost; }
		}

		public IList TemplateChoices
		{
			get { return _templateChoices; }
		}

		public bool TemplateChoicesVisible
		{
			get { return _templateChoices.Count > 1; }
		}

		public object FormatTemplate(object p)
		{
			var template = (TemplateData)p;
			return template.DisplayName;
		}

		public object SelectedTemplate
		{
			get { return _selectedTemplate; }
			set
			{
				if (!Equals(value, _selectedTemplate))
				{
					_selectedTemplate = (TemplateData)value;
					NotifyPropertyChanged("SelectedTemplate");
					NotifyPropertyChanged("IsOnBehalfOfEditable");
					InitializeFromTemplate(_selectedTemplate);
				}
			}
		}

		public string Body
		{
			get { return _body; }
			set
			{
				_body = value;
				NotifyPropertyChanged("Body");
			}
		}

		public bool IsPosting
		{
			get { return !string.IsNullOrEmpty(_body); }
		}

		public bool Urgent
		{
			get { return _urgent; }
			set { _urgent = value; }
		}

		public ICannedTextLookupHandler CannedTextLookupHandler
		{
			get { return _cannedTextLookupHandler; }
		}

		public IList OnBehalfOfGroupChoices
		{
			get
			{
				return _onBehalfOfChoices.ToList();
			}
		}

		public string FormatOnBehalfOf(object item)
		{
			var s = item == null ? null : ((StaffGroupSummary)item).Name;
			return s ?? "";
		}

		public StaffGroupSummary OnBehalfOf
		{
			get
			{
				return _onBehalfOf;
			}
			set
			{
				if (!Equals(value, _onBehalfOf))
				{
					_onBehalfOf = value;
					NotifyPropertyChanged("OnBehalfOf");
					OrderNoteConversationComponentSettings.Default.PreferredOnBehalfOfGroupName = _onBehalfOf != null ? _onBehalfOf.Name : string.Empty;
				}
			}
		}

		public bool IsOnBehalfOfEditable
		{
			get
			{
				// only editable if no template in effect
				return _selectedTemplate == null && this.IsPosting;
			}
		}

		public ITable Recipients
		{
			get { return _recipients; }
		}

		public ActionModelNode RecipientsActionModel
		{
			get { return _recipientsActionModel; }
		}

		public ISelection SelectedRecipient
		{
			get { return new Selection(_selectedRecipient); }
			set
			{
				if (value.Item != _selectedRecipient)
				{
					_selectedRecipient = (Checkable<RecipientTableItem>)value.Item;
				}

				OnSelectedRecipientChanged();
			}
		}

		public event EventHandler NewRecipientAdded
		{
			add { _newRecipientAdded += value; }
			remove { _newRecipientAdded -= value; }
		}

		public void AddRecipient()
		{
			// try to select an existing blank cell
			_selectedRecipient = CollectionUtils.SelectFirst(_recipients.Items, i => i.Item.Recipient == null);

			// if none, then add one
			if (_selectedRecipient == null)
			{
				_selectedRecipient = _recipients.AddNew(true);
			}

			// inform view to select the cell
			OnSelectedRecipientChanged();

			// inform view to begin editing the cell
			EventsHelper.Fire(_newRecipientAdded, this, EventArgs.Empty);
		}

		public bool SoftKeysVisible
		{
			get { return _softKeys.Count > 0; }
		}

		public IList<string> SoftKeyNames
		{
			get { return CollectionUtils.Map<SoftKeyData, string>(_softKeys, key => key.ButtonName); }
		}

		public void ApplySoftKey(string softKeyName)
		{
			var softKey = CollectionUtils.SelectFirst(_softKeys, key => Equals(key.ButtonName, softKeyName));
			this.Body = softKey.InsertText;
		}

		public string OrderNotesLabel
		{
			get
			{
				return _orderNoteViewComponent.HasAcknowledgeableNotes
					? SR.TitleConversationHistoryWithCheckBoxes
					: SR.TitleConversationHistory;
			}
		}

		public string CompleteButtonLabel
		{
			get
			{
				return _orderNoteViewComponent.HasAcknowledgeableNotes
						? (IsBodyEmpty ? SR.TitleAcknowledge : SR.TitleAcknowledgeAndPost)
						: SR.TitlePost;
			}
		}

		public void AcknowledgeAndPost()
		{
			if (this.HasValidationErrors)
			{
				this.ShowValidation(true);
				return;
			}

			if (!_orderNoteViewComponent.AllAcknowledgeableNotesAreChecked)
			{
				var mesage = this.IsBodyEmpty
					? SR.MessageMustAcknowledge
					: SR.MessageMustAcknowledgeWithPost;
				this.Host.DesktopWindow.ShowMessageBox(mesage, MessageBoxActions.Ok);
				return;
			}

			try
			{
				SaveChanges();

				Exit(ApplicationComponentExitCode.Accepted);
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, SR.ExceptionFailedToSave, this.Host.DesktopWindow,
										() => Exit(ApplicationComponentExitCode.Error));
			}
		}

		public void Cancel()
		{
			this.Exit(ApplicationComponentExitCode.None);
		}

		#endregion

		#region Private Methods

		private bool IsBodyEmpty
		{
			get { return string.IsNullOrEmpty(_body); }
		}

		private void UpdateSoftKeys()
		{
			_softKeys.Clear();

			// add default soft keys
			_softKeys.AddRange(_defaultSoftKeys);

			NotifyPropertyChanged("SoftKeyNames");
		}

		private void InitializeFromTemplate(TemplateData template)
		{
			var staffRecips = new List<StaffSummary>();
			var groupRecips = new List<StaffGroupSummary>();

			if(template != null)
			{
				var staffRecipIds = template.GetStaffRecipients();
				var groupRecipIds = template.GetGroupRecipients();
				if (staffRecipIds.Count > 0 || groupRecipIds.Count > 0)
				{
					// load the recipient staff/groups defined in the template
					Platform.GetService<IOrderNoteService>(
						service =>
						{
							var formDataRequest = new GetConversationEditorFormDataRequest(staffRecipIds, groupRecipIds);
							var formDataResponse = service.GetConversationEditorFormData(formDataRequest);
							staffRecips = formDataResponse.RecipientStaffs;
							groupRecips = formDataResponse.RecipientStaffGroups;
						});
				}
			}

			InitializeFromTemplate(template, staffRecips, groupRecips);
		}

		private void InitializeFromTemplate(TemplateData template,
			List<StaffSummary> templateStaffs,
			List<StaffGroupSummary> templateGroups)
		{
			InitializeOnBehalfOf(template);
			InitializeRecipients(template, templateStaffs, templateGroups);

			if(template != null)
			{
				// set note content
				this.Body = template.NoteContent;
			}
		}

		private void InitializeOnBehalfOf(TemplateData template)
		{
			if (template != null)
			{
				// take from template, and update the user prefs
				var groupName = template.OnBehalfOfGroup;
				OrderNoteConversationComponentSettings.Default.PreferredOnBehalfOfGroupName = StringUtilities.EmptyIfNull(groupName);
				OrderNoteConversationComponentSettings.Default.Save();

				_onBehalfOf = CollectionUtils.SelectFirst(_onBehalfOfChoices, group => group.Name == groupName);
			}
			else
			{
				// if not set from template, use the saved setting value
				_onBehalfOf = CollectionUtils.SelectFirst(_onBehalfOfChoices,
														  g => g.Name == OrderNoteConversationComponentSettings.Default.PreferredOnBehalfOfGroupName);
			}
		}

		private void InitializeRecipients(TemplateData template, List<StaffSummary> templateStaffs, List<StaffGroupSummary> templateGroups)
		{
			_recipients.Items.Clear();

			// add recipients from template if not null
			if (template != null)
			{
				foreach (var recipient in template.Recipients)
				{
					var staffOrGroup = recipient.Type == RecipientType.Staff ?
										(object)CollectionUtils.SelectFirst(templateStaffs, s => s.StaffId == recipient.Id)
										: CollectionUtils.SelectFirst(templateGroups, g => g.Name == recipient.Id);

					_recipients.Add(staffOrGroup, recipient.Mandatory, true);
				}
			}

			if(template == null || template.SuggestOtherRecipients)
			{
				// add additional recipients according to following algorithm

				// find all notes sent either directly to the current user, or to a group to which current user belongs
				var notesSentToCurrentUser =
					CollectionUtils.Select(_orderNotes,
										   n => CollectionUtils.Contains(n.StaffRecipients, sr => IsStaffCurrentUser(sr.Staff)) ||
												CollectionUtils.Contains(n.GroupRecipients, gr => _onBehalfOfChoices.Contains(gr.Group)));

				// get the set of senders (staff or groups) that posted notes to the current user
				var sendersOfNotesToCurrentUser =
					CollectionUtils.Map(notesSentToCurrentUser, (OrderNoteDetail n) => n.OnBehalfOfGroup ?? (object)n.Author);

				// remove the current user from this list
				sendersOfNotesToCurrentUser = CollectionUtils.Reject(sendersOfNotesToCurrentUser,
					sender => sender is StaffSummary && IsStaffCurrentUser((StaffSummary)sender));


				// add these senders, unchecked by default
				_recipients.AddRange(sendersOfNotesToCurrentUser, false, false);
			}
		}

		private static bool IsStaffCurrentUser(StaffSummary staff)
		{
			return string.Equals(staff.StaffId, LoginSession.Current.Staff.StaffId);
		}

		private void SaveChanges()
		{
			var orderNoteRefsToBeAcknowledged = CollectionUtils.Map(
				_orderNoteViewComponent.CheckedNotes, (OrderNoteDetail note) => note.OrderNoteRef);

			Platform.GetService<IOrderNoteService>(
				service => service.AcknowledgeAndPost(new AcknowledgeAndPostRequest(_orderRef, orderNoteRefsToBeAcknowledged, GetReply())));
		}

		private OrderNoteDetail GetReply()
		{
			// if Reply is unchecked or the body is empty, there is no reply to send.
			if (!IsBodyEmpty)
			{
				return new OrderNoteDetail(
					OrderNoteCategory.PreliminaryDiagnosis.Key,
					_body,
					_onBehalfOf == _emptyStaffGroup ? null : _onBehalfOf,
					_urgent,
					_recipients.CheckStaff,
					_recipients.CheckedStaffGroups);
			}
			return null;
		}

		private void DeleteRecipient()
		{
			if (_selectedRecipient == null)
				return;

			_recipients.Items.Remove(_selectedRecipient);
			this.SelectedRecipient = Selection.Empty;
		}

		private void OnSelectedRecipientChanged()
		{
			_recipientsActionModel.Delete.Enabled = _selectedRecipient != null && !_selectedRecipient.Item.IsMandatory;
			NotifyPropertyChanged("SelectedRecipient");
		}

		/// <summary>
		/// Aggregrates the set of recipients that are part of a conversation, performing some useful transformations along the way.
		/// </summary>
		/// <typeparam name="TRecip"></typeparam>
		/// <typeparam name="TOutput"></typeparam>
		/// <param name="orderNotes"></param>
		/// <param name="mapNoteToRecipList"></param>
		/// <param name="filterRecipList"></param>
		/// <param name="mapRecipToOutputType"></param>
		/// <param name="filterOutput"></param>
		/// <returns></returns>
		private static List<TOutput> AggregateRecipients<TRecip, TOutput>(
			List<OrderNoteDetail> orderNotes,
			Converter<OrderNoteDetail, List<TRecip>> mapNoteToRecipList,
			Predicate<TRecip> filterRecipList,
			Converter<TRecip, TOutput> mapRecipToOutputType,
			Predicate<TOutput> filterOutput)
			where TRecip : OrderNoteDetail.RecipientDetail
		{
			var recipList = CollectionUtils.Select(
								CollectionUtils.Concat(
									CollectionUtils.Map(orderNotes, mapNoteToRecipList)),
									filterRecipList);
			return CollectionUtils.Unique(
					CollectionUtils.Select(
						CollectionUtils.Map(recipList, mapRecipToOutputType),
						filterOutput));
		}

		#endregion
	}
}
