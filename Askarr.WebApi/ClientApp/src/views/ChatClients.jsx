/*!

=========================================================
* Argon Dashboard React - v1.0.0
=========================================================

* Product Page: https://www.creative-tim.com/product/argon-dashboard-react
* Copyright 2019 Creative Tim (https://www.creative-tim.com)
* Licensed under MIT (https://github.com/creativetimofficial/argon-dashboard-react/blob/master/LICENSE.md)

* Coded by Creative Tim

=========================================================

* The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

*/
import { useEffect, useState } from "react";
import { Oval } from 'react-loader-spinner'
import { useDispatch, useSelector } from 'react-redux';
import { Alert } from "reactstrap";
import { testSettings } from "../store/actions/ChatClientsActions"
import { testTelegramSettings } from "../store/actions/ChatClientsActions"
import { getSettings } from "../store/actions/ChatClientsActions"
import { save } from "../store/actions/ChatClientsActions"
import MultiDropdown from "../components/Inputs/MultiDropdown"
import Dropdown from "../components/Inputs/Dropdown"

// reactstrap components
import {
  Button,
  Card,
  CardHeader,
  CardBody,
  FormGroup,
  Form,
  Input,
  Container,
  Row,
  Col
} from "reactstrap";
// core components
import UserHeader from "../components/Headers/UserHeader.jsx";


function ChatClients(props) {
  const [isLoading, setIsLoading] = useState(true);
  const [isSaving, setIsSaving] = useState(false);
  const [saveAttempted, setSaveAttempted] = useState(false);
  const [saveSuccess, setSaveSuccess] = useState(false);
  const [saveError, setSaveError] = useState("");
  const [isCopyingLink, setIsCopyingLink] = useState(false);
  const [isTestingSettings, setIsTestingSettings] = useState(false);
  const [testSettingsRequested, setTestSettingsRequested] = useState(false);
  const [testSettingsSuccess, setTestSettingsSuccess] = useState(false);
  const [testSettingsError, setTestSettingsError] = useState("");
  const [monitoredChannels, setMonitoredChannels] = useState([]);
  const [statusMessage, setStatusMessage] = useState("");
  const [enableRequestsThroughDirectMessages, setEnableRequestsThroughDirectMessages] = useState(true);
  const [chatClient, setChatClient] = useState("");
  const [chatClientChanged, setChatClientChanged] = useState(false);
  const [chatClientInvalid, setChatClientInvalid] = useState(false);
  const [clientId, setClientId] = useState("");
  const [clientIdChanged, setClientIdChanged] = useState(false);
  const [clientIdInvalid, setClientIdInvalid] = useState(false);
  const [botToken, setBotToken] = useState("");
  const [botTokenChanged, setBotTokenChanged] = useState(false);
  const [botTokenInvalid, setBotTokenInvalid] = useState(false);
  const [telegramBotToken, setTelegramBotToken] = useState("");
  const [telegramBotTokenChanged, setTelegramBotTokenChanged] = useState(false);
  const [telegramBotTokenInvalid, setTelegramBotTokenInvalid] = useState(false);
  const [tvShowRoles, setTvShowRoles] = useState([]);
  const [movieRoles, setMovieRoles] = useState([]);
  const [musicRoles, setMusicRoles] = useState([]);
  const [automaticallyNotifyRequesters, setAutomaticallyNotifyRequesters] = useState(true);
  const [notificationMode, setNotificationMode] = useState("PrivateMessages");
  const [notificationChannels, setNotificationChannels] = useState([]);
  const [automaticallyPurgeCommandMessages, setAutomaticallyPurgeCommandMessages] = useState(true);
  const [language, setLanguage] = useState("english");
  const [availableLanguages, setAvailableLanguages] = useState([]);
  const [telegramMonitoredChats, setTelegramMonitoredChats] = useState([]);
  const [telegramNotificationChats, setTelegramNotificationChats] = useState([]);
  const [telegramMovieRoles, setTelegramMovieRoles] = useState([]);
  const [telegramTvRoles, setTelegramTvRoles] = useState([]);
  const [telegramMusicRoles, setTelegramMusicRoles] = useState([]);
  const [telegramEnableRequestsThroughDirectMessages, setTelegramEnableRequestsThroughDirectMessages] = useState(true);
  const [telegramAutomaticallyNotifyRequesters, setTelegramAutomaticallyNotifyRequesters] = useState(true);
  const [telegramNotificationMode, setTelegramNotificationMode] = useState("PrivateMessages");
  const [activeTab, setActiveTab] = useState("general");

  const dispatch = useDispatch();
  const userToken = useSelector(state => state.user.token);


  useEffect(() => {
    dispatch(getSettings(props.token))
      .then(data => {
        setIsLoading(false);
        setChatClient(data.payload.client);
        setClientId(data.payload.clientId);
        setBotToken(data.payload.botToken);
        setTelegramBotToken(data.payload.telegramBotToken);
        setStatusMessage(data.payload.statusMessage);
        setMonitoredChannels(data.payload.monitoredChannels);
        setEnableRequestsThroughDirectMessages(data.payload.enableRequestsThroughDirectMessages);
        setTvShowRoles(data.payload.tvShowRoles);
        setMovieRoles(data.payload.movieRoles);
        setMusicRoles(data.payload.musicRoles);
        setAutomaticallyNotifyRequesters(data.payload.automaticallyNotifyRequesters);
        setNotificationMode(data.payload.notificationMode);
        setNotificationChannels(data.payload.notificationChannels);
        setAutomaticallyPurgeCommandMessages(data.payload.automaticallyPurgeCommandMessages);
        setLanguage(data.payload.language);
        setAvailableLanguages(data.payload.availableLanguages);
        setTelegramMonitoredChats(data.payload.telegramMonitoredChats);
        setTelegramNotificationChats(data.payload.telegramNotificationChats || []);
        setTelegramMovieRoles(data.payload.telegramMovieRoles || []);
        setTelegramTvRoles(data.payload.telegramTvRoles || []);
        setTelegramMusicRoles(data.payload.telegramMusicRoles || []);
        setTelegramEnableRequestsThroughDirectMessages(data.payload.telegramEnableRequestsThroughDirectMessages);
        setTelegramAutomaticallyNotifyRequesters(data.payload.telegramAutomaticallyNotifyRequesters || true);
        setTelegramNotificationMode(data.payload.telegramNotificationMode || "PrivateMessages");
      });
  }, []);


  useEffect(() => {
    if (chatClientChanged)
      triggerChatClientValidation();
  }, [chatClientChanged]);


  useEffect(() => {
    if (clientIdChanged)
      triggerClientIdValidation()
  }, [clientIdChanged]);


  useEffect(() => {
    if (botTokenChanged)
      triggerBotTokenValidation()
  }, [botTokenChanged]);




  const validateChatClient = () => {
    return /\S/.test(chatClient);
  };

  const validateClientId = () => {
    return /\S/.test(clientId);
  };

  const validateBotToken = () => {
    return /\S/.test(botToken);
  };

  const validateTelegramBotToken = () => {
    return /\S/.test(telegramBotToken);
  };



  const onChatClientChange = (event) => {
    const newChatClient = event.target.value;
    setChatClient(newChatClient);
    setChatClientChanged(true);
    
    // Set appropriate active tab based on selected client
    if (newChatClient === "Discord") {
      setActiveTab("discord");
    } else if (newChatClient === "Telegram") {
      setActiveTab("telegram");
    } else if (newChatClient === "Discord,Telegram") {
      setActiveTab("general");
    } else {
      setActiveTab("general");
    }
  };

  const triggerChatClientValidation = () => {
    setChatClientInvalid(!validateChatClient());
  };

  const onStatusMessageChange = (event) => {
    setStatusMessage(event.target.value);
  };

  const onClientIdChange = (event) => {
    setClientId(event.target.value);
    setClientIdChanged(true);
  };


  const triggerClientIdValidation = () => {
    setClientIdInvalid(!validateClientId());
  };


  const onBotTokenChange = (event) => {
    setBotToken(event.target.value);
    setBotTokenChanged(true);
  };

  const triggerBotTokenValidation = () => {
    setBotTokenInvalid(!validateBotToken());
  };

  const onTelegramBotTokenChange = (event) => {
    setTelegramBotToken(event.target.value);
    setTelegramBotTokenChanged(true);
  };

  const triggerTelegramBotTokenValidation = () => {
    setTelegramBotTokenInvalid(!validateTelegramBotToken());
  };



  const onSaving = (e) => {
    e.preventDefault();

    triggerChatClientValidation();
    
    const isUsingDiscord = chatClient === "Discord" || chatClient === "Discord,Telegram";
    const isUsingTelegram = chatClient === "Telegram" || chatClient === "Discord,Telegram";
    
    let validationPassed = validateChatClient();
    
    if (isUsingDiscord) {
    triggerClientIdValidation();
    triggerBotTokenValidation();
      validationPassed = validationPassed && validateClientId() && validateBotToken();
    }
    
    if (isUsingTelegram) {
      triggerTelegramBotTokenValidation();
      validationPassed = validationPassed && validateTelegramBotToken();
    }

    if (!isSaving) {
      if (validationPassed) {
        setIsSaving(true);

        dispatch(save({
          client: chatClient,
          clientId: isUsingDiscord ? clientId : "",
          botToken: isUsingDiscord ? botToken : "",
          telegramBotToken: isUsingTelegram ? telegramBotToken : "",
          statusMessage: statusMessage,
          monitoredChannels: monitoredChannels,
          telegramMonitoredChats: telegramMonitoredChats,
          telegramNotificationChats: telegramNotificationChats,
          telegramMovieRoles: telegramMovieRoles,
          telegramTvRoles: telegramTvRoles,
          telegramMusicRoles: telegramMusicRoles,
          tvShowRoles: tvShowRoles,
          movieRoles: movieRoles,
          musicRoles: musicRoles,
          enableRequestsThroughDirectMessages: isUsingDiscord ? enableRequestsThroughDirectMessages : false,
          telegramEnableRequestsThroughDirectMessages: isUsingTelegram ? telegramEnableRequestsThroughDirectMessages : false,
          automaticallyNotifyRequesters: automaticallyNotifyRequesters,
          telegramAutomaticallyNotifyRequesters: telegramAutomaticallyNotifyRequesters,
          notificationMode: notificationMode,
          telegramNotificationMode: telegramNotificationMode,
          notificationChannels: notificationChannels,
          automaticallyPurgeCommandMessages: automaticallyPurgeCommandMessages,
          language: language,
        }))
          .then(data => {
            setIsSaving(false);

            if (data.ok) {
              setSaveAttempted(true);
              setSaveError("");
              setSaveSuccess(true);
            } else {
              let error = "An unknown error occurred while saving.";

              if (typeof (data.error) === "string")
                error = data.error;

              setSaveAttempted(true);
              setSaveError(error);
              setSaveSuccess(false);
            }
          });
      } else {
        setSaveAttempted(true);
        setSaveError("Some fields are invalid, please fix them before saving.");
        setSaveSuccess(false);
      }
    }
  }

  const onTestSettings = (e) => {
    e.preventDefault();

    triggerChatClientValidation();
    
    // Determine which service to test based on selected chat client and active tab
    const isTestingDiscord = (chatClient === "Discord" || chatClient === "Discord,Telegram") && 
                            (activeTab === "discord" || activeTab === "general");
    const isTestingTelegram = (chatClient === "Telegram" || chatClient === "Discord,Telegram") && 
                              (activeTab === "telegram" || activeTab === "general");
    
    let validationPassed = true;
    
    if (isTestingDiscord) {
    triggerClientIdValidation();
    triggerBotTokenValidation();
      validationPassed = validateChatClient() && validateBotToken() && validateClientId();
    } else if (isTestingTelegram) {
      triggerTelegramBotTokenValidation();
      validationPassed = validateChatClient() && validateTelegramBotToken();
    }
    
    if (!isTestingSettings) {
      if (validationPassed) {
      setIsTestingSettings(true);

        if (isTestingDiscord) {
          // Test Discord connection
      dispatch(testSettings({
            chatClient: "Discord",
        clientId: clientId,
        botToken: botToken,
      }))
        .then(data => {
          setIsTestingSettings(false);

          if (data.ok) {
            setTestSettingsRequested(true);
            setTestSettingsError("");
            setTestSettingsSuccess(true);
          } else {
                let error = "An unknown error occurred while testing Discord settings.";

            if (typeof (data.error) === "string")
              error = data.error;

            setTestSettingsRequested(true);
            setTestSettingsError(error);
            setTestSettingsSuccess(false);
          }
        });
        } else if (isTestingTelegram) {
          // Test Telegram connection
          dispatch({
            type: 'TEST_TELEGRAM_SETTINGS_REQUEST',
            payload: {
              chatClient: "Telegram",
              botToken: telegramBotToken,
            }
          });

          dispatch(testTelegramSettings({
            chatClient: "Telegram",
            botToken: telegramBotToken,
          }))
          .then(data => {
            setIsTestingSettings(false);

            if (data.ok) {
              setTestSettingsRequested(true);
              setTestSettingsError("");
              setTestSettingsSuccess(true);
            } else {
              let error = "An unknown error occurred while testing Telegram settings.";

              if (typeof (data.error) === "string")
                error = data.error;

              setTestSettingsRequested(true);
              setTestSettingsError(error);
              setTestSettingsSuccess(false);
            }
          })
          .catch(error => {
            setIsTestingSettings(false);
            setTestSettingsRequested(true);
            setTestSettingsError("Failed to test Telegram connection: " + error.message);
            setTestSettingsSuccess(false);
          });
        }
      } else {
        setTestSettingsRequested(true);
        setTestSettingsError("Some fields are invalid, please fix them before testing.");
        setTestSettingsSuccess(false);
      }
    }
  }

  const onGenerateInviteLink = (e) => {
    e.preventDefault();

    triggerChatClientValidation();
    triggerClientIdValidation();
    triggerBotTokenValidation();

    if (!isCopyingLink
      && validateChatClient()
      && validateBotToken()
      && validateClientId()) {
      setIsCopyingLink(true);

      let linkElement = document.getElementById("discordlink");
      linkElement.classList.remove("d-none");
      linkElement.focus();
      linkElement.select();
      let text = linkElement.value;
      navigator.clipboard.writeText(text);
      linkElement.classList.add("d-none");

      // let thisRef = this;
      setTimeout(() => { setIsCopyingLink(false) }, 3000);
    }
  };





  return (
    <>
      <UserHeader
        title="Chat Clients"
        description="Configure your bot to interact with Discord and Telegram."
      />
      <Container className="mt--7" fluid>
            <Card className="bg-secondary shadow">
              <CardHeader className="bg-white border-0">
                <Row className="align-items-center">
                  <Col xs="8">
                <h3 className="mb-0">Chat Bot Configuration</h3>
                  </Col>
                </Row>
              </CardHeader>
          <CardBody>
            {isLoading && (
              <div className="text-center">
                <Oval color="#5e72e4" secondaryColor="#5e72e4" height={40} width={40} />
              </div>
            )}

            {saveAttempted && (
              <Alert
                color={saveSuccess ? "success" : "danger"}
                toggle={() => setSaveAttempted(false)}
              >
                {saveSuccess
                  ? "Settings have been saved successfully."
                  : saveError}
              </Alert>
            )}

            {testSettingsRequested && (
              <Alert
                color={testSettingsSuccess ? "success" : "danger"}
                toggle={() => setTestSettingsRequested(false)}
              >
                {testSettingsSuccess
                  ? "Test was successful! Your bot token is valid."
                  : testSettingsError}
              </Alert>
            )}

            {!isLoading && (
              <Form onSubmit={(e) => e.preventDefault()}>
                <h6 className="heading-small text-muted mb-4">General Settings</h6>
                  <div className="pl-lg-4">
                    <Row>
                      <Col lg="6">
                        <FormGroup>
                          <label
                            className="form-control-label"
                          htmlFor="input-chat-client"
                          >
                          Chat Client*
                          </label>
                          <Input
                          className="form-control-alternative"
                          id="input-chat-client"
                          placeholder="Select a chat client"
                          type="select"
                            value={chatClient}
                            onChange={onChatClientChange}
                          invalid={chatClientInvalid}
                          >
                          <option value="">Select a chat client</option>
                            <option value="Discord">Discord</option>
                            <option value="Telegram">Telegram</option>
                          <option value="Discord,Telegram">Both Discord & Telegram</option>
                          </Input>
                        </FormGroup>
                      </Col>
                    <Col lg="6">
                      <FormGroup>
                        <label className="form-control-label" htmlFor="input-language">
                          Language
                        </label>
                        <Dropdown
                          id="input-language"
                          placeholder="Select a language"
                          options={availableLanguages.map(x => ({ label: x, value: x.toLowerCase() }))}
                          value={language}
                          onSelect={({ value }) => setLanguage(value)}
                        />
                      </FormGroup>
                    </Col>
                    </Row>
                  </div>
                
                {/* Navigation Tabs */}
                <div className="nav-tabs-wrapper mt-5">
                  <ul className="nav nav-tabs">
                    <li className="nav-item">
                      <a 
                        className={`nav-link ${activeTab === "general" ? "active" : ""}`}
                        onClick={() => setActiveTab("general")}
                        href="#general"
                        role="tab"
                        data-toggle="tab"
                      >
                        General
                      </a>
                    </li>
                    {(chatClient === "Discord" || chatClient === "Discord,Telegram") && (
                      <li className="nav-item">
                        <a 
                          className={`nav-link ${activeTab === "discord" ? "active" : ""}`}
                          onClick={() => setActiveTab("discord")}
                          href="#discord"
                          role="tab"
                          data-toggle="tab"
                        >
                          Discord
                        </a>
                      </li>
                    )}
                    {(chatClient === "Telegram" || chatClient === "Discord,Telegram") && (
                      <li className="nav-item">
                        <a 
                          className={`nav-link ${activeTab === "telegram" ? "active" : ""}`}
                          onClick={() => setActiveTab("telegram")}
                          href="#telegram"
                          role="tab"
                          data-toggle="tab"
                        >
                          Telegram
                        </a>
                      </li>
                    )}
                  </ul>
                </div>
                
                <div className="tab-content">
                  {/* General Tab */}
                  <div className={`tab-pane ${activeTab === "general" ? "active" : ""}`} id="general">
                    <h6 className="heading-small text-muted mb-4">Bot Status</h6>
                    <div className="pl-lg-4">
                      <Row>
                        <Col lg="12">
                          <FormGroup>
                            <label
                              className="form-control-label"
                              htmlFor="input-status-message"
                            >
                              Status Message
                            </label>
                            <Input
                              className="form-control-alternative"
                              id="input-status-message"
                              placeholder="Set your bot status message"
                              type="text"
                              value={statusMessage}
                              onChange={onStatusMessageChange}
                            />
                        </FormGroup>
                      </Col>
                    </Row>
                    </div>
                  </div>
                  
                  {/* Discord Tab */}
                  {(chatClient === "Discord" || chatClient === "Discord,Telegram") && (
                    <div className={`tab-pane ${activeTab === "discord" ? "active" : ""}`} id="discord">
                      <h6 className="heading-small text-muted mb-4">Discord Configuration</h6>
                  <div className="pl-lg-4">
                    <Row>
                      <Col lg="6">
                            <FormGroup>
                          <label
                            className="form-control-label"
                            htmlFor="input-client-id"
                          >
                                Client ID*
                          </label>
                          <Input
                            className="form-control-alternative"
                            id="input-client-id"
                                placeholder="Discord client ID"
                            type="text"
                                value={clientId}
                                onChange={onClientIdChange}
                                invalid={clientIdInvalid}
                              />
                        </FormGroup>
                      </Col>
                      <Col lg="6">
                            <FormGroup>
                          <label
                            className="form-control-label"
                            htmlFor="input-bot-token"
                          >
                                Bot Token*
                          </label>
                          <Input
                            className="form-control-alternative"
                            id="input-bot-token"
                                placeholder="Discord bot token"
                                type="password"
                                value={botToken}
                                onChange={onBotTokenChange}
                                invalid={botTokenInvalid}
                              />
                            </FormGroup>
                          </Col>
                        </Row>
                        
                        <Row className="mb-4">
                          <Col lg="12">
                            <Button
                              color="primary"
                              size="sm"
                              type="button"
                              onClick={onGenerateInviteLink}
                              disabled={isCopyingLink}
                            >
                              {isCopyingLink ? "Copied!" : "Copy Invite Link"}
                            </Button>
                            <Input
                              id="discordlink"
                            type="text"
                              className="d-none"
                              value={`https://discord.com/oauth2/authorize?client_id=${clientId}&scope=bot%20applications.commands&permissions=8`}
                              readOnly
                            />
                            <small className="text-muted d-block mt-2">
                              Use this link to invite your bot to your Discord server
                            </small>
                          </Col>
                        </Row>
                        
                        <Row>
                          <Col lg="12">
                            <FormGroup>
                              <label className="form-control-label" htmlFor="input-notification-mode">
                                Notification Mode
                              </label>
                              <Input
                                className="form-control-alternative"
                                id="input-notification-mode"
                                placeholder="Select notification mode"
                                type="select"
                                value={notificationMode}
                                onChange={(e) => setNotificationMode(e.target.value)}
                              >
                                <option value="PrivateMessages">Private Messages</option>
                                <option value="Channels">Channels</option>
                              </Input>
                        </FormGroup>
                      </Col>
                    </Row>
                        
                        <Row>
                          <Col lg="12">
                            <FormGroup>
                              <div className="custom-control custom-checkbox mb-3">
                                <input
                                  className="custom-control-input"
                                  id="check-private-responses"
                                  type="checkbox"
                                  checked={automaticallyPurgeCommandMessages}
                                  onChange={(e) => setAutomaticallyPurgeCommandMessages(e.target.checked)}
                                />
                                <label
                                  className="custom-control-label"
                                  htmlFor="check-private-responses"
                                >
                                  Make command responses only visible to you (private responses)
                                </label>
                              </div>
                            </FormGroup>
                          </Col>
                        </Row>
                        
                        {/* Continue with existing Discord settings */}
                    <Row>
                      <Col lg="6">
                        <FormGroup>
                          <MultiDropdown
                            name="Roles allowed to request tv shows"
                            create={true}
                            searchable={true}
                            placeholder="Enter role ids here. Leave blank for all roles."
                            labelField="name"
                            valueField="id"
                            dropdownHandle={false}
                            selectedItems={tvShowRoles.map(x => { return { name: x, id: x } })}
                            items={tvShowRoles.map(x => { return { name: x, id: x } })}
                            onChange={newTvShowRoles => setTvShowRoles(newTvShowRoles.filter(x => /\S/.test(x.id)).map(x => x.id.trim()))} />
                        </FormGroup>
                      </Col>
                      <Col lg="6">
                        <FormGroup>
                          <MultiDropdown
                            name="Roles allowed to request movies"
                            create={true}
                            searchable={true}
                            placeholder="Enter role ids here. Leave blank for all roles."
                            labelField="name"
                            valueField="id"
                            dropdownHandle={false}
                            selectedItems={movieRoles.map(x => { return { name: x, id: x } })}
                            items={movieRoles.map(x => { return { name: x, id: x } })}
                            onChange={newMovieRoles => setMovieRoles(newMovieRoles.filter(x => /\S/.test(x.id)).map(x => x.id.trim()))} />
                        </FormGroup>
                      </Col>
                      <Col lg="6">
                        <FormGroup>
                          <MultiDropdown
                            name="Roles allowed to request music"
                            create={true}
                            searchable={true}
                            placeholder="Enter role ids here. Leave blank for all roles."
                            labelField="name"
                            valueField="id"
                            dropdownHandle={false}
                            selectedItems={musicRoles.map(x => { return { name: x, id: x } })}
                            items={musicRoles.map(x => { return { name: x, id: x } })}
                            onChange={newMusicRoles => setMusicRoles(newMusicRoles.filter(x => /\S/.test(x.id)).map(x => x.id.trim()))} />
                        </FormGroup>
                      </Col>
                    </Row>
                    <Row>
                      <Col md="12">
                        <FormGroup>
                          <MultiDropdown
                            name="Channel(s) to monitor"
                            create={true}
                            searchable={true}
                            placeholder="Enter channels ids here. Leave blank for all channels."
                            labelField="name"
                            valueField="id"
                            dropdownHandle={false}
                            selectedItems={monitoredChannels.map(x => { return { name: x, id: x } })}
                            items={monitoredChannels.map(x => { return { name: x, id: x } })}
                            onChange={newMonitoredChannels => setMonitoredChannels(newMonitoredChannels.filter(x => /\S/.test(x.id)).map(x => x.id.trim().replace(/#/g, '').replace(/\s+/g, '-')))} />
                        </FormGroup>
                      </Col>
                    </Row>
                  </div>
                    </div>
                  )}

                  {/* Telegram Tab */}
                  {(chatClient === "Telegram" || chatClient === "Discord,Telegram") && (
                    <div className={`tab-pane ${activeTab === "telegram" ? "active" : ""}`} id="telegram">
                      <h6 className="heading-small text-muted mb-4">Telegram Configuration</h6>
                      <div className="pl-lg-4">
                    <Row>
                          <Col lg="12">
                            <Alert color="info" className="mb-4">
                              <h4 className="alert-heading"><i className="ni ni-bell-55 mr-2"></i>Getting Started with Telegram</h4>
                              <ol className="mb-0">
                                <li>Create a bot on Telegram using <a href="https://t.me/BotFather" target="_blank" rel="noopener noreferrer">@BotFather</a></li>
                                <li>Copy the API token BotFather gives you and paste it below</li>
                                <li>Find your bot on Telegram by searching for its username</li>
                                <li>Start a chat with your bot by clicking "Start" or sending "/start"</li>
                                <li>Add your bot to groups where you want it to respond to commands</li>
                                <li>To get chat IDs, use <a href="https://t.me/RawDataBot" target="_blank" rel="noopener noreferrer">@RawDataBot</a> in your chats</li>
                              </ol>
                            </Alert>
                          </Col>
                        </Row>
                        <Row>
                          <Col lg="12">
                        <FormGroup>
                          <label
                            className="form-control-label"
                                htmlFor="input-telegram-bot-token"
                          >
                                Telegram Bot Token*
                          </label>
                          <Input
                            className="form-control-alternative"
                                id="input-telegram-bot-token"
                                placeholder="Telegram bot token"
                                type="password"
                                value={telegramBotToken}
                                onChange={onTelegramBotTokenChange}
                                invalid={telegramBotTokenInvalid}
                              />
                              <small className="text-muted">
                                Obtain your bot token from @BotFather on Telegram
                              </small>
                        </FormGroup>
                      </Col>
                    </Row>
                        
                        <Row className="mt-4">
                          <Col lg="12">
                            <h6 className="heading-small text-muted mb-3">Bot Behavior</h6>
                      </Col>
                    </Row>
                        
                          <Row>
                          <Col lg="6">
                            <FormGroup>
                              <div className="custom-control custom-checkbox mb-3">
                                <input
                                  className="custom-control-input"
                                  id="check-enable-telegram-dms"
                                  type="checkbox"
                                  checked={telegramEnableRequestsThroughDirectMessages}
                                  onChange={(e) => setTelegramEnableRequestsThroughDirectMessages(e.target.checked)}
                                />
                                <label
                                  className="custom-control-label"
                                  htmlFor="check-enable-telegram-dms"
                                >
                                  Enable requests through direct messages
                                </label>
                              </div>
                              </FormGroup>
                            </Col>
                          <Col lg="6">
                            <FormGroup>
                              <div className="custom-control custom-checkbox mb-3">
                                <input
                            className="custom-control-input"
                                  id="check-telegram-auto-notify"
                            type="checkbox"
                                  checked={telegramAutomaticallyNotifyRequesters}
                                  onChange={(e) => setTelegramAutomaticallyNotifyRequesters(e.target.checked)}
                          />
                          <label
                            className="custom-control-label"
                                  htmlFor="check-telegram-auto-notify"
                          >
                                  Automatically notify requesters
                          </label>
                              </div>
                        </FormGroup>
                      </Col>
                    </Row>
                        
                        <Row className="mt-4">
                          <Col lg="12">
                            <h6 className="heading-small text-muted mb-3">Chats & Permissions</h6>
                          </Col>
                        </Row>
                        
                    <Row>
                          <Col lg="12">
                            <FormGroup>
                              <label className="form-control-label" htmlFor="input-telegram-monitored-chats">
                                Monitored Chat IDs
                              </label>
                              <div className="mb-2">
                          <Input
                                  type="textarea"
                                  className="form-control-alternative"
                                  id="input-telegram-monitored-chats"
                                  placeholder="Enter chat IDs, one per line"
                                  rows="3"
                                  value={telegramMonitoredChats.join('\n')}
                                  onChange={(e) => setTelegramMonitoredChats(e.target.value.split('\n').map(id => id.trim()).filter(id => id))}
                          />
                              </div>
                              <small className="text-muted">
                                Enter the chat IDs where your bot should respond to commands. Use @RawDataBot in a chat to get its ID.
                                <br />Group chat IDs typically start with a minus sign (e.g., -1001234567890).
                              </small>
                        </FormGroup>
                      </Col>
                    </Row>
                        
                        <Row className="mt-4">
                          <Col lg="12">
                            <h6 className="heading-small text-muted mb-3">Notification Settings</h6>
                      </Col>
                    </Row>
                        
                    <Row>
                      <Col lg="6">
                            <FormGroup>
                              <label className="form-control-label" htmlFor="input-telegram-notification-mode">
                                Notification Mode
                              </label>
                              <Input
                                className="form-control-alternative"
                                id="input-telegram-notification-mode"
                                placeholder="Select notification mode"
                                type="select"
                                value={telegramNotificationMode}
                                onChange={(e) => setTelegramNotificationMode(e.target.value)}
                              >
                                <option value="PrivateMessages">Private Messages</option>
                                <option value="Channels">Channels/Groups</option>
                                <option value="Both">Both</option>
                              </Input>
                            </FormGroup>
                          </Col>
                          <Col lg="6">
                            <FormGroup>
                              <label className="form-control-label" htmlFor="input-telegram-notification-chats">
                                Notification Chats
                              </label>
                              <div className="mb-2">
                              <Input
                                  type="textarea"
                                className="form-control-alternative"
                                  id="input-telegram-notification-chats"
                                  placeholder="Enter notification chat IDs, one per line"
                                  rows="3"
                                  value={telegramNotificationChats.join('\n')}
                                  onChange={(e) => setTelegramNotificationChats(e.target.value.split('\n').map(id => id.trim()).filter(id => id))}
                                />
                              </div>
                              <small className="text-muted">
                                Enter the chat IDs where notifications should be sent.
                              </small>
                            </FormGroup>
                          </Col>
                        </Row>
                        
                        <Row className="mt-4">
                          <Col lg="12">
                            <h6 className="heading-small text-muted mb-3">Access Control</h6>
                      </Col>
                    </Row>
                        
                    <Row>
                          <Col lg="4">
                            <FormGroup>
                              <label className="form-control-label" htmlFor="input-telegram-movie-roles">
                                Users with Movie Access
                              </label>
                              <Input
                                type="textarea"
                                className="form-control-alternative"
                                id="input-telegram-movie-roles"
                                placeholder="Enter user IDs, one per line"
                                rows="3"
                                value={telegramMovieRoles.join('\n')}
                                onChange={(e) => setTelegramMovieRoles(e.target.value.split('\n').map(id => id.trim()).filter(id => id))}
                              />
                              <small className="text-muted">
                                Leave empty to allow all users
                              </small>
                            </FormGroup>
                          </Col>
                          <Col lg="4">
                                  <FormGroup>
                              <label className="form-control-label" htmlFor="input-telegram-tv-roles">
                                Users with TV Show Access
                              </label>
                              <Input
                                type="textarea"
                                className="form-control-alternative"
                                id="input-telegram-tv-roles"
                                placeholder="Enter user IDs, one per line"
                                rows="3"
                                value={telegramTvRoles.join('\n')}
                                onChange={(e) => setTelegramTvRoles(e.target.value.split('\n').map(id => id.trim()).filter(id => id))}
                              />
                              <small className="text-muted">
                                Leave empty to allow all users
                              </small>
                                  </FormGroup>
                          </Col>
                          <Col lg="4">
                            <FormGroup>
                              <label className="form-control-label" htmlFor="input-telegram-music-roles">
                                Users with Music Access
                              </label>
                                    <Input
                                type="textarea"
                                className="form-control-alternative"
                                id="input-telegram-music-roles"
                                placeholder="Enter user IDs, one per line"
                                rows="3"
                                value={telegramMusicRoles.join('\n')}
                                onChange={(e) => setTelegramMusicRoles(e.target.value.split('\n').map(id => id.trim()).filter(id => id))}
                              />
                              <small className="text-muted">
                                Leave empty to allow all users
                              </small>
                                  </FormGroup>
                                </Col>
                              </Row>
                        
                        <Row className="mt-3">
                          <Col lg="12">
                            <Alert color="warning">
                              <strong>Note:</strong> Telegram doesn't support role-based permissions like Discord. 
                              Access control is managed by user ID. You'll need to get user IDs by having them interact with your bot.
                            </Alert>
                          </Col>
                        </Row>
                      </div>
                    </div>
                  )}
                </div>

                  <hr className="my-4" />
                  
                <Row className="align-items-center">
                  <Col className="text-right" lg="12">
                    {chatClient && (
                      <Button
                        color="info"
                        size="md"
                        type="button"
                        onClick={onTestSettings}
                        disabled={isTestingSettings}
                      >
                        {isTestingSettings ? (
                          <>
                            <span className="btn-inner--icon">
                              <Oval color="#FFF" height={15} width={15} />
                            </span>
                            <span className="btn-inner--text ml-2">Testing...</span>
                          </>
                        ) : (
                          "Test Connection"
                        )}
                      </Button>
                    )}
                    <Button
                      color="primary"
                      size="md"
                      type="button"
                      onClick={onSaving}
                      disabled={isSaving}
                      className="ml-2"
                    >
                      {isSaving ? (
                        <>
                          <span className="btn-inner--icon">
                            <Oval color="#FFF" height={15} width={15} />
                          </span>
                          <span className="btn-inner--text ml-2">Saving...</span>
                        </>
                      ) : (
                        "Save Settings"
                      )}
                    </Button>
                      </Col>
                    </Row>
                </Form>
            )}
              </CardBody>
            </Card>
      </Container>
    </>
  );
}

export default ChatClients;
