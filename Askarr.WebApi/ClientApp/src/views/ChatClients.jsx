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
import { useDispatch, useSelector } from 'react-redux';
import { Alert } from "reactstrap";
import { testSettings, testTelegramSettings, getSettings, save } from "../store/actions/ChatClientsActions";
import MultiDropdown from "../components/Inputs/MultiDropdown";
import Dropdown from "../components/Inputs/Dropdown";
import ClientCard from "../components/Cards/ClientCard";
import ModernHeader from "../components/Headers/ModernHeader";

// reactstrap components
import {
  Button,
  Card,
  CardHeader,
  CardBody,
  Form,
  Input,
  Container,
  Row,
  Col,
  Nav,
  NavItem,
  NavLink,
  TabContent,
  TabPane,
  FormGroup,
  InputGroup,
  InputGroupAddon,
  InputGroupText,
  Spinner
} from "reactstrap";

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
  const [isTestingBottomSettings, setIsTestingBottomSettings] = useState(false);
  const [testBottomSettingsRequested, setTestBottomSettingsRequested] = useState(false);
  const [testBottomSettingsSuccess, setTestBottomSettingsSuccess] = useState(false);
  const [testBottomSettingsError, setTestBottomSettingsError] = useState("");
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
  const [telegramNotificationChats, setTelegramNotificationChats] = useState([]);
  const [telegramMovieRoles, setTelegramMovieRoles] = useState([]);
  const [telegramTvRoles, setTelegramTvRoles] = useState([]);
  const [telegramMusicRoles, setTelegramMusicRoles] = useState([]);
  const [telegramEnableRequestsThroughDirectMessages, setTelegramEnableRequestsThroughDirectMessages] = useState(true);
  const [telegramAutomaticallyNotifyRequesters, setTelegramAutomaticallyNotifyRequesters] = useState(true);
  const [telegramNotificationMode, setTelegramNotificationMode] = useState("PrivateMessages");
  const [activeTab, setActiveTab] = useState("general");
  const [language, setLanguage] = useState("english");
  const [availableLanguages, setAvailableLanguages] = useState([]);
  const [telegramMonitoredChats, setTelegramMonitoredChats] = useState([]);
  const [tvShowRoles, setTvShowRoles] = useState([]);
  const [movieRoles, setMovieRoles] = useState([]);
  const [musicRoles, setMusicRoles] = useState([]);
  const [automaticallyNotifyRequesters, setAutomaticallyNotifyRequesters] = useState(true);
  const [notificationMode, setNotificationMode] = useState("PrivateMessages");
  const [notificationChannels, setNotificationChannels] = useState([]);
  const [automaticallyPurgeCommandMessages, setAutomaticallyPurgeCommandMessages] = useState(true);
  const [usePrivateResponses, setUsePrivateResponses] = useState(true);

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
        setUsePrivateResponses(data.payload.usePrivateResponses !== undefined ? data.payload.usePrivateResponses : true);
        setLanguage(data.payload.language ? data.payload.language.toLowerCase() : "english");
        setAvailableLanguages(data.payload.availableLanguages || []);
        setTelegramMonitoredChats(data.payload.telegramMonitoredChats);
        setTelegramNotificationChats(data.payload.telegramNotificationChats || []);
        setTelegramMovieRoles(data.payload.telegramMovieRoles || []);
        setTelegramTvRoles(data.payload.telegramTvRoles || []);
        setTelegramMusicRoles(data.payload.telegramMusicRoles || []);
        setTelegramEnableRequestsThroughDirectMessages(data.payload.telegramEnableRequestsThroughDirectMessages);
        setTelegramAutomaticallyNotifyRequesters(data.payload.telegramAutomaticallyNotifyRequesters || true);
        setTelegramNotificationMode(data.payload.telegramNotificationMode || "PrivateMessages");
        
        // Set appropriate active tab based on selected client
        if (data.payload.client === "Discord") {
          setActiveTab("discord");
        } else if (data.payload.client === "Telegram") {
          setActiveTab("telegram");
        } else {
          setActiveTab("general");
        }
      });
  }, []);

  useEffect(() => {
    if (chatClientChanged)
      triggerChatClientValidation();
  }, [chatClientChanged]);

  useEffect(() => {
    if (clientIdChanged)
      triggerClientIdValidation();
  }, [clientIdChanged]);

  useEffect(() => {
    if (botTokenChanged)
      triggerBotTokenValidation();
  }, [botTokenChanged]);

  // Add auto-dismiss timeouts for all notification types
  useEffect(() => {
    if (saveAttempted) {
      const timer = setTimeout(() => {
        setSaveAttempted(false);
      }, 5000);
      return () => clearTimeout(timer);
    }
  }, [saveAttempted]);

  useEffect(() => {
    if (testSettingsRequested) {
      const timer = setTimeout(() => {
        setTestSettingsRequested(false);
      }, 5000);
      return () => clearTimeout(timer);
    }
  }, [testSettingsRequested]);

  useEffect(() => {
    if (testBottomSettingsRequested) {
      const timer = setTimeout(() => {
        setTestBottomSettingsRequested(false);
      }, 5000);
      return () => clearTimeout(timer);
    }
  }, [testBottomSettingsRequested]);

  // Validation functions
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

  // Event handlers
  const onChatClientChange = (newClient) => {
    setChatClient(newClient);
    setChatClientChanged(true);
    
    // Set appropriate active tab based on selected client
    if (newClient === "Discord") {
      setActiveTab("discord");
    } else if (newClient === "Telegram") {
      setActiveTab("telegram");
    } else if (newClient === "Discord,Telegram") {
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

  const onToggleTab = (tab) => {
    if (activeTab !== tab) {
      setActiveTab(tab);
    }
  };

  const onSaving = (e) => {
    e.preventDefault();

    if (!isSaving) {
      setIsSaving(true);
      setSaveAttempted(false); // Reset save attempt state

      const saveModel = {
        client: chatClient,
        clientId: clientId,
        botToken: botToken,
        statusMessage: statusMessage,
        monitoredChannels: monitoredChannels,
        tvShowRoles: tvShowRoles,
        movieRoles: movieRoles,
        musicRoles: musicRoles,
        enableRequestsThroughDirectMessages: enableRequestsThroughDirectMessages,
        automaticallyNotifyRequesters: automaticallyNotifyRequesters,
        notificationMode: notificationMode,
        notificationChannels: notificationChannels,
        automaticallyPurgeCommandMessages: automaticallyPurgeCommandMessages,
        usePrivateResponses: usePrivateResponses,
        telegramBotToken: telegramBotToken,
        telegramMonitoredChats: telegramMonitoredChats,
        telegramNotificationChats: telegramNotificationChats,
        telegramMovieRoles: telegramMovieRoles,
        telegramTvRoles: telegramTvRoles,
        telegramMusicRoles: telegramMusicRoles,
        telegramEnableRequestsThroughDirectMessages: telegramEnableRequestsThroughDirectMessages,
        telegramAutomaticallyNotifyRequesters: telegramAutomaticallyNotifyRequesters,
        telegramNotificationMode: telegramNotificationMode,
        language: language
      };
    
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

      if (validationPassed) {
        dispatch(save(saveModel))
          .then(result => {
            setIsSaving(false);
              setSaveAttempted(true);

            if (result.ok) {
              setSaveError("");
              setSaveSuccess(true);
            } else {
              let error = "An unknown error occurred while saving.";

              if (typeof (result.error) === "string") {
                error = result.error;
              } else if (result.error && typeof (result.error) === "object") {
                error = JSON.stringify(result.error);
              }

              setSaveError(error);
              setSaveSuccess(false);
            }
          })
          .catch(err => {
            setIsSaving(false);
            setSaveAttempted(true);
            setSaveSuccess(false);
            setSaveError("Network error: " + err.message);
            console.error("Save error:", err);
          });
      } else {
        setIsSaving(false);
        setSaveAttempted(true);
        setSaveError("Some fields are invalid, please fix them before saving.");
        setSaveSuccess(false);
      }
    }
  };

  const onTestSettings = (e) => {
    e.preventDefault();
    
    if (!isTestingSettings) {
      setIsTestingSettings(true);
      setTestSettingsRequested(true);
      console.log("Testing Discord settings...");

      dispatch(testSettings({
        botToken: botToken
      })).then(data => {
          setIsTestingSettings(false);
        console.log("Test results:", data);

          if (data.ok) {
            setTestSettingsSuccess(true);
          setTestSettingsError("");
          console.log("Discord settings test successful");
        }
        else {
          let error = "An unknown error occurred while testing the settings.";

          if (typeof (data.error) === "string") {
              error = data.error;
          } else if (data.error && typeof (data.error) === "object") {
            error = JSON.stringify(data.error);
          }

            setTestSettingsSuccess(false);
          setTestSettingsError(error);
          console.log("Discord settings test failed:", error);
        }
      }).catch(err => {
        setIsTestingSettings(false);
        setTestSettingsSuccess(false);
        setTestSettingsError("Network error: " + err.message);
        console.error("Discord test error:", err);
      });
    }
  };
  
  const onBottomTestSettings = (e) => {
    e.preventDefault();

    if (!isTestingBottomSettings) {
      setIsTestingBottomSettings(true);
      setTestBottomSettingsRequested(true);
      console.log("Testing Discord settings from bottom...");

      dispatch(testSettings({
        botToken: botToken
      })).then(data => {
        setIsTestingBottomSettings(false);
        console.log("Test results:", data);

        if (data.ok) {
          setTestBottomSettingsSuccess(true);
          setTestBottomSettingsError("");
          console.log("Discord settings test successful");
        }
        else {
          let error = "An unknown error occurred while testing the settings.";

          if (typeof (data.error) === "string") {
            error = data.error;
          } else if (data.error && typeof (data.error) === "object") {
            error = JSON.stringify(data.error);
          }

          setTestBottomSettingsSuccess(false);
          setTestBottomSettingsError(error);
          console.log("Discord settings test failed:", error);
        }
      }).catch(err => {
        setIsTestingBottomSettings(false);
        setTestBottomSettingsSuccess(false);
        setTestBottomSettingsError("Network error: " + err.message);
        console.error("Discord test error:", err);
      });
    }
  };

  const onTestTelegramSettings = (e) => {
    e.preventDefault();

    if (!isTestingSettings) {
      setIsTestingSettings(true);
      setTestSettingsRequested(true);
      console.log("Testing Telegram settings...");

          dispatch(testTelegramSettings({
        telegramBotToken: telegramBotToken
      })).then(data => {
            setIsTestingSettings(false);
        console.log("Telegram test results:", data);

            if (data.ok) {
              setTestSettingsSuccess(true);
          setTestSettingsError("");
          console.log("Telegram settings test successful");
        }
        else {
          let error = "An unknown error occurred while testing the settings.";

          if (typeof (data.error) === "string") {
                error = data.error;
          } else if (data.error && typeof (data.error) === "object") {
            error = JSON.stringify(data.error);
          }

              setTestSettingsSuccess(false);
          setTestSettingsError(error);
          console.log("Telegram settings test failed:", error);
            }
      }).catch(err => {
            setIsTestingSettings(false);
            setTestSettingsSuccess(false);
        setTestSettingsError("Network error: " + err.message);
        console.error("Telegram test error:", err);
      });
    }
  };
  
  const onBottomTestTelegramSettings = (e) => {
    e.preventDefault();

    if (!isTestingBottomSettings) {
      setIsTestingBottomSettings(true);
      setTestBottomSettingsRequested(true);
      console.log("Testing Telegram settings from bottom...");

      dispatch(testTelegramSettings({
        telegramBotToken: telegramBotToken
      })).then(data => {
        setIsTestingBottomSettings(false);
        console.log("Telegram test results:", data);

        if (data.ok) {
          setTestBottomSettingsSuccess(true);
          setTestBottomSettingsError("");
          console.log("Telegram settings test successful");
        }
        else {
          let error = "An unknown error occurred while testing the settings.";

          if (typeof (data.error) === "string") {
            error = data.error;
          } else if (data.error && typeof (data.error) === "object") {
            error = JSON.stringify(data.error);
          }

          setTestBottomSettingsSuccess(false);
          setTestBottomSettingsError(error);
          console.log("Telegram settings test failed:", error);
        }
      }).catch(err => {
        setIsTestingBottomSettings(false);
        setTestBottomSettingsSuccess(false);
        setTestBottomSettingsError("Network error: " + err.message);
        console.error("Telegram test error:", err);
      });
    }
  };

  return (
    <>
      <ModernHeader
        title="Chat Clients"
        description="Configure connection between your bot and chat platforms"
        icon="fas fa-comments"
      />
      
      <Container className="mt--7" fluid>
        <Row>
          <Col className="order-xl-1" xl="12">
            <Card className="modern-card shadow">
              <CardHeader className="bg-white border-0">
                <Row className="align-items-center">
                  <Col>
                    <h3 className="mb-0">Chat Platforms</h3>
                    <p className="text-sm text-muted mb-0">
                      Select which chat platforms to use for your bot
                    </p>
                  </Col>
                </Row>
              </CardHeader>
              
              <CardBody className={isLoading ? "fade" : "fade show"}>
                {isLoading ? (
                  <div className="text-center py-4">
                    <Spinner color="primary" />
                  </div>
                ) : (
              <Form onSubmit={(e) => e.preventDefault()}>
                    <Row>
                      <Col lg="3">
                        <ClientCard
                          title="Discord"
                          description="Connect to Discord for chat integration"
                          icon="fab fa-discord"
                          isActive={chatClient === "Discord"}
                          onClick={() => onChatClientChange("Discord")}
                          color="primary"
                        />
                      </Col>
                      <Col lg="3">
                        <ClientCard
                          title="Telegram"
                          description="Connect to Telegram for chat integration"
                          icon="fab fa-telegram"
                          isActive={chatClient === "Telegram"}
                          onClick={() => onChatClientChange("Telegram")}
                          color="info"
                        />
                      </Col>
                      <Col lg="3">
                        <ClientCard
                          title="Both"
                          description="Use both Discord and Telegram"
                          icon="fas fa-globe"
                          isActive={chatClient === "Discord,Telegram"}
                          onClick={() => onChatClientChange("Discord,Telegram")}
                          color="success"
                        />
                      </Col>
                      <Col lg="3">
                        <ClientCard
                          title="Disabled"
                          description="No chat integration"
                          icon="fas fa-ban"
                          isActive={chatClient === ""}
                          onClick={() => onChatClientChange("")}
                          color="secondary"
                        />
                    </Col>
                    </Row>
                    
                    {chatClient && (
                      <>
                        <hr className="my-4" />
                        <Nav tabs className="mb-4">
                          {chatClient === "Discord,Telegram" && (
                            <NavItem>
                              <NavLink
                                className={activeTab === "general" ? "active" : ""}
                                onClick={() => onToggleTab("general")}
                                style={{ cursor: "pointer" }}
                              >
                                <i className="fas fa-cog mr-2"></i>
                        General
                              </NavLink>
                            </NavItem>
                          )}
                          
                    {(chatClient === "Discord" || chatClient === "Discord,Telegram") && (
                            <NavItem>
                              <NavLink
                                className={activeTab === "discord" ? "active" : ""}
                                onClick={() => onToggleTab("discord")}
                                style={{ cursor: "pointer" }}
                              >
                                <i className="fab fa-discord mr-2"></i>
                          Discord
                              </NavLink>
                            </NavItem>
                    )}
                          
                    {(chatClient === "Telegram" || chatClient === "Discord,Telegram") && (
                            <NavItem>
                              <NavLink
                                className={activeTab === "telegram" ? "active" : ""}
                                onClick={() => onToggleTab("telegram")}
                                style={{ cursor: "pointer" }}
                              >
                                <i className="fab fa-telegram mr-2"></i>
                          Telegram
                              </NavLink>
                            </NavItem>
                          )}
                        </Nav>
                        
                        <TabContent activeTab={activeTab}>
                          <TabPane tabId="general">
                      <Row>
                              <Col md="6">
                          <FormGroup>
                                  <label className="form-control-label">Status Message</label>
                            <Input
                              type="text"
                              value={statusMessage}
                              onChange={onStatusMessageChange}
                                    placeholder="Status message"
                                  />
                                  <small className="form-text text-muted">
                                    This will be displayed as the bot's status message
                                  </small>
                                </FormGroup>
                              </Col>
                              <Col md="6">
                                <FormGroup>
                                  <label className="form-control-label">Language</label>
                                  <Dropdown
                                    name="Language"
                                    value={language}
                                    items={availableLanguages.map(lang => ({ name: lang, value: lang.toLowerCase() }))}
                                    onChange={newLanguage => setLanguage(newLanguage)}
                                  />
                                  <small className="form-text text-muted">
                                    The language used by the bot
                                  </small>
                        </FormGroup>
                      </Col>
                    </Row>
                          </TabPane>
                          
                          <TabPane tabId="discord">
                            <Row className="mb-4">
                              <Col lg="12">
                                <Card className="border-0 shadow-sm mb-3">
                                  <CardBody>
                                    <h5 className="text-primary mb-3">
                                      <i className="fab fa-discord mr-2"></i>
                                      Bot Configuration
                                    </h5>
                    <Row>
                                      <Col md="6">
                            <FormGroup>
                                          <label className="form-control-label">Client ID</label>
                          <Input
                            type="text"
                                value={clientId}
                                onChange={onClientIdChange}
                                invalid={clientIdInvalid}
                                            placeholder="Enter Discord client ID"
                              />
                                          <small className="form-text text-muted">
                                            Found in your Discord Developer Portal
                                          </small>
                        </FormGroup>
                      </Col>
                                      <Col md="6">
                            <FormGroup>
                                          <label className="form-control-label">Bot Token</label>
                          <Input
                                type="password"
                                value={botToken}
                                onChange={onBotTokenChange}
                                invalid={botTokenInvalid}
                                            placeholder="Enter Discord bot token"
                              />
                                          <small className="form-text text-muted">
                                            Used to authenticate your bot
                                          </small>
                            </FormGroup>
                          </Col>
                        </Row>
                                    <Row>
                                      <Col className="text-right">
                            <Button
                              color="primary"
                                          className="modern-btn"
                                          onClick={() => {
                                            if (clientId) {
                                              window.open(`https://discord.com/api/oauth2/authorize?client_id=${clientId}&permissions=8&scope=bot%20applications.commands`, '_blank');
                                            } else {
                                              alert('Please enter a Client ID first');
                                            }
                                          }}
                                        >
                                          <i className="fas fa-plus-circle mr-2"></i>
                                          Invite Bot to Server
                            </Button>
                                      </Col>
                                    </Row>
                                  </CardBody>
                                </Card>
                              </Col>
                            </Row>

                            <Row className="mb-4">
                              <Col lg="12">
                                <Card className="border-0 shadow-sm mb-3">
                                  <CardBody>
                                    <h5 className="text-primary mb-3">
                                      <i className="fas fa-cog mr-2"></i>
                                      Channel Settings
                                    </h5>
                                    <Row>
                                      <Col md="12" className="mb-3">
                                        <FormGroup>
                                          <label className="form-control-label">Command Channels</label>
                            <Input
                                            type="textarea"
                                            rows="2"
                                            value={monitoredChannels.join("\n")}
                                            onChange={(e) => setMonitoredChannels(e.target.value.split("\n").filter(c => c.trim()))}
                                            placeholder="Enter channel IDs (one per line)"
                                          />
                                          <small className="form-text text-muted">
                                            Channels where bot will respond to commands
                            </small>
                                        </FormGroup>
                          </Col>
                        </Row>
                        <Row>
                                      <Col md="6">
                            <FormGroup>
                                          <label className="form-control-label">Notification Mode</label>
                              <Input
                                type="select"
                                value={notificationMode}
                                onChange={(e) => setNotificationMode(e.target.value)}
                              >
                                <option value="PrivateMessages">Private Messages</option>
                                <option value="Channels">Channels</option>
                              </Input>
                                          <small className="form-text text-muted">
                                            How notifications will be delivered
                                          </small>
                        </FormGroup>
                      </Col>
                                      <Col md="6">
                            <FormGroup>
                                          <label className="form-control-label">Notification Channels</label>
                                          <Input
                                            type="textarea"
                                            rows="2"
                                            value={notificationChannels.join("\n")}
                                            onChange={(e) => setNotificationChannels(e.target.value.split("\n").filter(c => c.trim()))}
                                            placeholder="Enter channel IDs (one per line)"
                                          />
                                          <small className="form-text text-muted">
                                            Channels for sending notifications
                                          </small>
                            </FormGroup>
                                      </Col>
                                    </Row>
                                  </CardBody>
                                </Card>
                          </Col>
                        </Row>
                        
                            <Row className="mb-4">
                              <Col lg="12">
                                <Card className="border-0 shadow-sm mb-3">
                                  <CardBody>
                                    <h5 className="text-primary mb-3">
                                      <i className="fas fa-user-lock mr-2"></i>
                                      Permission Settings
                                    </h5>
                    <Row>
                                      <Col md="4">
                        <FormGroup>
                                          <label className="form-control-label">Movie Access Roles</label>
                                          <Input
                                            type="textarea"
                                            rows="2"
                                            value={movieRoles.join("\n")}
                                            onChange={(e) => setMovieRoles(e.target.value.split("\n").filter(r => r.trim()))}
                                            placeholder="Enter role IDs (one per line)"
                                          />
                                          <small className="form-text text-muted">
                                            Leave empty to allow all users
                                          </small>
                        </FormGroup>
                      </Col>
                                      <Col md="4">
                        <FormGroup>
                                          <label className="form-control-label">TV Show Access Roles</label>
                                          <Input
                                            type="textarea"
                                            rows="2"
                                            value={tvShowRoles.join("\n")}
                                            onChange={(e) => setTvShowRoles(e.target.value.split("\n").filter(r => r.trim()))}
                                            placeholder="Enter role IDs (one per line)"
                                          />
                                          <small className="form-text text-muted">
                                            Leave empty to allow all users
                                          </small>
                        </FormGroup>
                      </Col>
                                      <Col md="4">
                        <FormGroup>
                                          <label className="form-control-label">Music Access Roles</label>
                          <Input
                                            type="textarea"
                                            rows="2"
                                            value={musicRoles.join("\n")}
                                            onChange={(e) => setMusicRoles(e.target.value.split("\n").filter(r => r.trim()))}
                                            placeholder="Enter role IDs (one per line)"
                                          />
                                          <small className="form-text text-muted">
                                            Leave empty to allow all users
                              </small>
                        </FormGroup>
                      </Col>
                    </Row>
                                  </CardBody>
                                </Card>
                      </Col>
                    </Row>
                        
                            <Row className="mb-4">
                              <Col lg="12">
                                <Card className="border-0 shadow-sm mb-3">
                                  <CardBody>
                                    <h5 className="text-primary mb-3">
                                      <i className="fas fa-toggle-on mr-2"></i>
                                      Behavior Settings
                                    </h5>
                          <Row>
                                      <Col md="4">
                                        <div className="custom-control custom-switch mb-3">
                                <input
                                  className="custom-control-input"
                                            id="enableRequestsThroughDirectMessages"
                                  type="checkbox"
                                            checked={enableRequestsThroughDirectMessages}
                                            onChange={e => setEnableRequestsThroughDirectMessages(e.target.checked)}
                                          />
                                          <label className="custom-control-label" htmlFor="enableRequestsThroughDirectMessages">
                                            Allow DM requests
                                </label>
                              </div>
                            </Col>
                                      <Col md="4">
                                        <div className="custom-control custom-switch mb-3">
                                <input
                            className="custom-control-input"
                                            id="usePrivateResponses"
                            type="checkbox"
                                            checked={usePrivateResponses}
                                            onChange={e => setUsePrivateResponses(e.target.checked)}
                                          />
                                          <label className="custom-control-label" htmlFor="usePrivateResponses">
                                            Use private responses
                          </label>
                                          <div className="small text-muted mt-1">
                                            When checked, bot responses show "Only you can see this".<br/>
                                            Unchecking makes responses visible to everyone.
                              </div>
                                        </div>
                      </Col>
                    </Row>
                                  </CardBody>
                                </Card>
                          </Col>
                        </Row>
                          </TabPane>
                        
                          <TabPane tabId="telegram">
                            <Row className="mb-4">
                          <Col lg="12">
                                <Card className="border-0 shadow-sm mb-3">
                                  <CardBody>
                                    <h5 className="text-primary mb-3">
                                      <i className="fab fa-telegram mr-2"></i>
                                      Bot Configuration
                                    </h5>
                                    <Row>
                                      <Col md="12">
                            <FormGroup>
                                          <label className="form-control-label">Bot Token</label>
                          <Input
                                            type="password"
                                            value={telegramBotToken}
                                            onChange={onTelegramBotTokenChange}
                                            invalid={telegramBotTokenInvalid}
                                            placeholder="Enter Telegram bot token"
                                          />
                                          <small className="form-text text-muted">
                                            Get this from @BotFather on Telegram
                              </small>
                        </FormGroup>
                                      </Col>
                                    </Row>
                                    <Row>
                                      <Col className="text-right">
                                        <Button
                                          color="primary"
                                          className="modern-btn"
                                          onClick={() => {
                                            // Open Telegram BotFather
                                            window.open('https://t.me/botfather', '_blank');
                                          }}
                                        >
                                          <i className="fas fa-robot mr-2"></i>
                                          Create Bot with @BotFather
                                        </Button>
                                      </Col>
                                    </Row>
                                  </CardBody>
                                </Card>
                      </Col>
                    </Row>
                        
                            <Row className="mb-4">
                          <Col lg="12">
                                <Card className="border-0 shadow-sm mb-3">
                                  <CardBody>
                                    <h5 className="text-primary mb-3">
                                      <i className="fas fa-comment mr-2"></i>
                                      Chat Settings
                                    </h5>
                                    <Row>
                                      <Col md="6" className="mb-3">
                                        <FormGroup>
                                          <label className="form-control-label">Command Chats</label>
                                          <Input
                                            type="textarea"
                                            rows="2"
                                            value={telegramMonitoredChats.join("\n")}
                                            onChange={(e) => setTelegramMonitoredChats(e.target.value.split("\n").filter(c => c.trim()))}
                                            placeholder="Enter chat IDs (one per line)"
                                          />
                                          <small className="form-text text-muted">
                                            Telegram chats where bot responds to commands
                                          </small>
                                        </FormGroup>
                                      </Col>
                                      <Col md="6" className="mb-3">
                                        <FormGroup>
                                          <label className="form-control-label">Notification Chats</label>
                                          <Input
                                            type="textarea"
                                            rows="2"
                                            value={telegramNotificationChats.join("\n")}
                                            onChange={(e) => setTelegramNotificationChats(e.target.value.split("\n").filter(c => c.trim()))}
                                            placeholder="Enter chat IDs (one per line)"
                                          />
                                          <small className="form-text text-muted">
                                            Telegram chats for sending notifications
                                          </small>
                                        </FormGroup>
                      </Col>
                    </Row>
                    <Row>
                                      <Col md="6">
                            <FormGroup>
                                          <label className="form-control-label">Notification Mode</label>
                              <Input
                                type="select"
                                value={telegramNotificationMode}
                                onChange={(e) => setTelegramNotificationMode(e.target.value)}
                              >
                                <option value="PrivateMessages">Private Messages</option>
                                            <option value="Channels">Channels</option>
                                <option value="Both">Both</option>
                              </Input>
                                          <small className="form-text text-muted">
                                            How notifications will be delivered
                              </small>
                            </FormGroup>
                          </Col>
                        </Row>
                                  </CardBody>
                                </Card>
                      </Col>
                    </Row>
                        
                            <Row className="mb-4">
                              <Col lg="12">
                                <Card className="border-0 shadow-sm mb-3">
                                  <CardBody>
                                    <h5 className="text-primary mb-3">
                                      <i className="fas fa-user-lock mr-2"></i>
                                      User Access Settings
                                    </h5>
                    <Row>
                                      <Col md="4">
                            <FormGroup>
                                          <label className="form-control-label">Movie Access</label>
                              <Input
                                type="textarea"
                                            rows="2"
                                            value={telegramMovieRoles.join("\n")}
                                            onChange={(e) => setTelegramMovieRoles(e.target.value.split("\n").filter(r => r.trim()))}
                                            placeholder="Enter user IDs (one per line)"
                                          />
                                          <small className="form-text text-muted">
                                            Users who can request movies
                              </small>
                            </FormGroup>
                          </Col>
                                      <Col md="4">
                                  <FormGroup>
                                          <label className="form-control-label">TV Show Access</label>
                              <Input
                                type="textarea"
                                            rows="2"
                                            value={telegramTvRoles.join("\n")}
                                            onChange={(e) => setTelegramTvRoles(e.target.value.split("\n").filter(r => r.trim()))}
                                            placeholder="Enter user IDs (one per line)"
                                          />
                                          <small className="form-text text-muted">
                                            Users who can request TV shows
                              </small>
                                  </FormGroup>
                          </Col>
                                      <Col md="4">
                            <FormGroup>
                                          <label className="form-control-label">Music Access</label>
                                    <Input
                                type="textarea"
                                            rows="2"
                                            value={telegramMusicRoles.join("\n")}
                                            onChange={(e) => setTelegramMusicRoles(e.target.value.split("\n").filter(r => r.trim()))}
                                            placeholder="Enter user IDs (one per line)"
                                          />
                                          <small className="form-text text-muted">
                                            Users who can request music
                              </small>
                                  </FormGroup>
                                      </Col>
                                    </Row>
                                  </CardBody>
                                </Card>
                                </Col>
                              </Row>
                        
                            <Row className="mb-4">
                          <Col lg="12">
                                <Card className="border-0 shadow-sm mb-3">
                                  <CardBody>
                                    <h5 className="text-primary mb-3">
                                      <i className="fas fa-toggle-on mr-2"></i>
                                      Behavior Settings
                                    </h5>
                                    <Row>
                                      <Col md="6">
                                        <div className="custom-control custom-switch mb-3">
                                          <input
                                            className="custom-control-input"
                                            id="telegramEnableRequestsThroughDirectMessages"
                                            type="checkbox"
                                            checked={telegramEnableRequestsThroughDirectMessages}
                                            onChange={e => setTelegramEnableRequestsThroughDirectMessages(e.target.checked)}
                                          />
                                          <label className="custom-control-label" htmlFor="telegramEnableRequestsThroughDirectMessages">
                                            Allow DM requests
                                          </label>
                                        </div>
                          </Col>
                                      <Col md="6">
                                        <div className="custom-control custom-switch mb-3">
                                          <input
                                            className="custom-control-input"
                                            id="telegramAutomaticallyNotifyRequesters"
                                            type="checkbox"
                                            checked={telegramAutomaticallyNotifyRequesters}
                                            onChange={e => setTelegramAutomaticallyNotifyRequesters(e.target.checked)}
                                          />
                                          <label className="custom-control-label" htmlFor="telegramAutomaticallyNotifyRequesters">
                                            Auto-notify requesters
                                          </label>
                      </div>
                                      </Col>
                                    </Row>
                                  </CardBody>
                                </Card>
                              </Col>
                            </Row>
                          </TabPane>
                        </TabContent>
                      </>
                    )}
                    
                    <Row className="mt-4">
                      <Col>
                        {saveAttempted && !isSaving && (
                          saveSuccess ? (
                            <Alert className="text-center fade-alert" color="success">
                              <strong>Settings updated successfully.</strong>
                            </Alert>
                          ) : (
                            <Alert className="text-center fade-alert" color="danger">
                              <strong>{saveError}</strong>
                            </Alert>
                          )
                        )}
                        
                        <div className="text-right">
                          {activeTab === "discord" && (
                            <>
                      <Button
                        color="info"
                                className="modern-btn mr-2"
                                onClick={onBottomTestSettings}
                                disabled={isSaving || isTestingBottomSettings}
                              >
                                {isTestingBottomSettings ? (
                                  <>
                                    <Spinner size="sm" className="mr-2" />
                                    Testing...
                                  </>
                                ) : (
                                  <>
                                    <i className="fas fa-vial mr-2"></i>
                                    Test Discord Settings
                                  </>
                                )}
                              </Button>
                              {testBottomSettingsRequested && !isTestingBottomSettings && (
                                <span className="mr-2 fade-notification">
                                  {testBottomSettingsSuccess ? (
                                    <span className="text-success">
                                      <i className="fas fa-check-circle mr-1"></i>
                                      Test successful!
                            </span>
                                  ) : (
                                    <span className="text-danger">
                                      <i className="fas fa-times-circle mr-1"></i>
                                      Test failed
                                    </span>
                                  )}
                                </span>
                              )}
                            </>
                          )}
                          
                          {activeTab === "telegram" && (
                            <>
                              <Button
                                color="info"
                                className="modern-btn mr-2"
                                onClick={onBottomTestTelegramSettings}
                                disabled={isSaving || isTestingBottomSettings}
                              >
                                {isTestingBottomSettings ? (
                                  <>
                                    <Spinner size="sm" className="mr-2" />
                                    Testing...
                          </>
                        ) : (
                                  <>
                                    <i className="fas fa-vial mr-2"></i>
                                    Test Telegram Settings
                                  </>
                        )}
                      </Button>
                              {testBottomSettingsRequested && !isTestingBottomSettings && (
                                <span className="mr-2 fade-notification">
                                  {testBottomSettingsSuccess ? (
                                    <span className="text-success">
                                      <i className="fas fa-check-circle mr-1"></i>
                                      Test successful!
                                    </span>
                                  ) : (
                                    <span className="text-danger">
                                      <i className="fas fa-times-circle mr-1"></i>
                                      Test failed
                                    </span>
                                  )}
                                </span>
                              )}
                            </>
                          )}

                    <Button
                      color="primary"
                      className="modern-btn"
                      onClick={onSaving}
                      disabled={isSaving}
                      type="button"
                    >
                      {isSaving ? (
                        <>
                          <Spinner size="sm" className="mr-2" />
                          Saving...
                        </>
                      ) : (
                        <>
                          <i className="fas fa-save mr-2"></i>
                          Save Changes
                        </>
                      )}
                    </Button>
                        </div>
                      </Col>
                    </Row>
                </Form>
            )}
              </CardBody>
            </Card>
          </Col>
        </Row>
      </Container>
    </>
  );
}

export default ChatClients;
