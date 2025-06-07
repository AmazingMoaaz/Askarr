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
import { getSettings } from "../store/actions/TvShowsClientsActions"
import { saveDisabledClient } from "../store/actions/TvShowsClientsActions"
import { saveSonarrClient } from "../store/actions/SonarrClientActions"
import { saveOmbiClient } from "../store/actions/TvShowsClientsActions"
import { saveOverseerrTvShowClient as saveOverseerrClient } from "../store/actions/OverseerrClientSonarrActions"
import Dropdown from "../components/Inputs/Dropdown"
import Sonarr from "../components/DownloadClients/Sonarr/Sonarr"
import Ombi from "../components/DownloadClients/Ombi"
import Overseerr from "../components/DownloadClients/Overseerr/TvShows/OverseerrTvShow"
import ClientCard from "../components/Cards/ClientCard"
import ModernHeader from "../components/Headers/ModernHeader"

// reactstrap components
import {
  Button,
  Card,
  CardHeader,
  CardBody,
  Form,
  Container,
  Row,
  Col,
  Spinner
} from "reactstrap";

function TvShows() {
  const [isSubmitted, setIsSubmitted] = useState(false);
  const [isLoading, setIsLoading] = useState(true);
  const [isSaving, setIsSaving] = useState(false);
  const [saveAttempted, setSaveAttempted] = useState(false);
  const [saveSuccess, setSaveSuccess] = useState(false);
  const [saveError, setSaveError] = useState("");
  const [client, setClient] = useState("");
  const [restrictions, setRestrictions] = useState("None");
  const [sonarr, setSonarr] = useState({});
  const [isSonarrValid, setIsSonarrValid] = useState(false);
  const [ombi, setOmbi] = useState({});
  const [isOmbiValid, setIsOmbiValid] = useState(false);
  const [overseerr, setOverseerr] = useState({});
  const [isOverseerrValid, setIsOverseerrValid] = useState(false);

  const reduxState = useSelector((state) => {
    return {
      settings: state.tvShows
    }
  });
  const dispatch = useDispatch();

  useEffect(() => {
    dispatch(getSettings())
      .then((data) => {
        setIsLoading(false);
        setClient(data.payload.client);
        setSonarr(data.payload.sonarr);
        setOmbi(data.payload.ombi);
        setOverseerr(data.payload.overseerr);
        setRestrictions(data.payload.restrictions);
      });
  }, []);

  useEffect(() => {
    if (!isSubmitted) return;
    
    if (!isSaving) {
      if ((client === "Disabled"
        || (client === "Sonarr" && isSonarrValid)
        || (client === "Ombi" && isOmbiValid)
        || (client === "Overseerr" && isOverseerrValid)
      )) {
        setIsSaving(true);

        let saveAction = null;

        if (client === "Disabled") {
          saveAction = dispatch(saveDisabledClient());
        }
        else if (client === "Ombi") {
          saveAction = dispatch(saveOmbiClient({
            ombi: ombi,
            restrictions: restrictions
          }));
        }
        else if (client === "Overseerr") {
          saveAction = dispatch(saveOverseerrClient({
            overseerr: overseerr,
            restrictions: restrictions
          }));
        }
        else if (client === "Sonarr") {
          saveAction = dispatch(saveSonarrClient({
            sonarr: sonarr,
            restrictions: restrictions
          }));
        }

        saveAction.then(data => {
          setIsSaving(false);
          setIsSubmitted(false);

          if (data.ok) {
            setSaveAttempted(true);
            setSaveError("");
            setSaveSuccess(true);
          }
          else {
            let error = "An unknown error occurred while saving.";

            if (typeof (data.error) === "string")
              error = data.error;

            setSaveAttempted(true);
            setSaveError(error);
            setSaveSuccess(false);
          }
        });
      }
      else {
        setSaveAttempted(true);
        setSaveError("Some fields are invalid, please fix them before saving.");
        setSaveSuccess(false);
        setIsSubmitted(false);
      }
    }
  }, [isSubmitted]);

  const onClientChange = (newClient) => {
    setClient(newClient);
    setSonarr(reduxState.settings.sonarr);
    setOmbi(reduxState.settings.ombi);
    setOverseerr(reduxState.settings.overseerr);
    setSaveAttempted(false);
    setIsSubmitted(false);
  };

  const onSaving = (e) => {
    e.preventDefault();
    setIsSubmitted(true);
  };

  const renderClientConfig = () => {
    if (client === "Disabled") return null;
    
    if (client === "Sonarr") {
      return (
        <Sonarr 
          onChange={newSonarr => setSonarr(newSonarr)} 
          onValidate={newIsSonarrValid => setIsSonarrValid(newIsSonarrValid)} 
          isSubmitted={isSubmitted} 
          isSaving={isSaving} 
        />
      );
    }
    
    if (client === "Ombi") {
      return (
        <Ombi 
          type={"tvshow"} 
          settings={ombi} 
          onChange={newOmbi => setOmbi(newOmbi)} 
          onValidate={newIsOmbiValid => setIsOmbiValid(newIsOmbiValid)} 
          isSubmitted={isSubmitted} 
        />
      );
    }
    
    if (client === "Overseerr") {
      return (
        <Overseerr 
          onChange={newOverseerr => setOverseerr(newOverseerr)} 
          onValidate={newIsOverseerrValid => setIsOverseerrValid(newIsOverseerrValid)} 
          isSubmitted={isSubmitted} 
          isSaving={isSaving} 
        />
      );
    }
    
    return null;
  }

  return (
    <>
      <ModernHeader 
        title="TV Shows" 
        description="Configure connection between your bot and TV shows download client"
        icon="fas fa-tv"
      />
      
      <Container className="mt--7" fluid>
        <Row>
          <Col className="order-xl-1" xl="12">
            <Card className="modern-card shadow">
              <CardHeader className="bg-white border-0">
                <Row className="align-items-center">
                  <Col>
                    <h3 className="mb-0">Download Client</h3>
                    <p className="text-sm text-muted mb-0">
                      Select a download client for TV show requests
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
                  <Form className="modern-form">
                    <Row>
                      <Col lg="3">
                        <ClientCard
                          title="Disabled"
                          description="No TV show download client"
                          icon="fas fa-ban"
                          isActive={client === "Disabled"}
                          onClick={() => onClientChange("Disabled")}
                          color="secondary"
                        />
                      </Col>
                      <Col lg="3">
                        <ClientCard
                          title="Sonarr"
                          description="Connect to Sonarr for TV show management"
                          icon="fas fa-tv"
                          isActive={client === "Sonarr"}
                          onClick={() => onClientChange("Sonarr")}
                          color="primary"
                        />
                      </Col>
                      <Col lg="3">
                        <ClientCard
                          title="Overseerr"
                          description="Connect to Overseerr for TV show requests"
                          icon="fas fa-server"
                          isActive={client === "Overseerr"}
                          onClick={() => onClientChange("Overseerr")}
                          color="info"
                        />
                      </Col>
                      <Col lg="3">
                        <ClientCard
                          title="Ombi"
                          description="Connect to Ombi for TV show requests"
                          icon="fas fa-database"
                          isActive={client === "Ombi"}
                          onClick={() => onClientChange("Ombi")}
                          color="warning"
                        />
                      </Col>
                    </Row>
                    
                    {reduxState.settings.client !== client && reduxState.settings.client !== "Disabled" && (
                      <Row className="mt-4">
                        <Col>
                          <Alert className="text-center" color="warning">
                            <strong>Changing the download client will delete all pending TV show notifications.</strong>
                          </Alert>
                        </Col>
                      </Row>
                    )}
                    
                    {client !== "Disabled" && (
                      <>
                        <hr className="my-4" />
                        <Row>
                          <Col md="6" lg="4">
                            <div className="form-group">
                              <label className="form-control-label">Season Restrictions</label>
                              <Dropdown
                                name="Season Restrictions"
                                value={restrictions}
                                items={[
                                  { name: "No restrictions", value: "None" }, 
                                  { name: "Force all seasons", value: "AllSeasons" }, 
                                  { name: "Force single season", value: "SingleSeason" }
                                ]}
                                onChange={newRestrictions => { setRestrictions(newRestrictions) }}
                              />
                            </div>
                      </Col>
                    </Row>
                        {renderClientConfig()}
                      </>
                    )}
                    
                    <Row className="mt-4">
                      <Col>
                        {saveAttempted && !isSaving && (
                          saveSuccess ? (
                            <Alert className="text-center" color="success">
                              <strong>Settings updated successfully.</strong>
                            </Alert>
                          ) : (
                                <Alert className="text-center" color="danger">
                                  <strong>{saveError}</strong>
                                </Alert>
                          )
                        )}
                        
                        <div className="text-right">
                          <Button
                            color="primary"
                            className="modern-btn"
                            onClick={onSaving}
                            disabled={isSaving}
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

export default TvShows;