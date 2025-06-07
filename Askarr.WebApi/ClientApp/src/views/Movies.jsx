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
import { getSettings } from "../store/actions/MovieClientsActions"
import { saveDisabledClient } from "../store/actions/MovieClientsActions"
import { saveRadarrClient } from "../store/actions/RadarrClientActions"
import { saveOmbiClient } from "../store/actions/MovieClientsActions"
import { saveOverseerrMovieClient as saveOverseerrClient } from "../store/actions/OverseerrClientRadarrActions"
import Dropdown from "../components/Inputs/Dropdown"
import Radarr from "../components/DownloadClients/Radarr/Radarr"
import Ombi from "../components/DownloadClients/Ombi"
import Overseerr from "../components/DownloadClients/Overseerr/Movies/OverseerrMovie"
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
// core components
import UserHeader from "../components/Headers/UserHeader.jsx";


function Movies() {
  const [isSubmitted, setIsSubmitted] = useState(false);
  const [isLoading, setIsLoading] = useState(true);
  const [isSaving, setIsSaving] = useState(false);
  const [saveAttempted, setSaveAttempted] = useState(false);
  const [saveSuccess, setSaveSuccess] = useState(false);
  const [saveError, setSaveError] = useState("");
  const [client, setClient] = useState("");
  const [radarr, setRadarr] = useState({});
  const [isRadarrValid, setIsRadarrValid] = useState(false);
  const [ombi, setOmbi] = useState({});
  const [isOmbiValid, setIsOmbiValid] = useState(false);
  const [overseerr, setOverseerr] = useState({});
  const [isOverseerrValid, setIsOverseerrValid] = useState(false);


  const reduxState = useSelector((state) => {
    return {
      settings: state.movies
    }
  });
  const dispatch = useDispatch();


  useEffect(() => {
    dispatch(getSettings())
      .then(data => {
        setIsLoading(false);
        setClient(data.payload.client);
        setRadarr(data.payload.radarr);
        setOmbi(data.payload.ombi);
        setOverseerr(data.payload.overseerr);
      });
  }, []);
 

  useEffect(() => {
    if (!isSubmitted)
      return;

    if (!isSaving) {
      if ((client === "Disabled"
        || (client === "Radarr"
          && isRadarrValid)
        || (client === "Ombi"
          && isOmbiValid)
        || (client === "Overseerr"
          && isOverseerrValid)
      )) {
        setIsSaving(true);

        let saveAction = null;

        if (client === "Disabled") {
          saveAction = dispatch(saveDisabledClient());
        }
        else if (client === "Ombi") {
          saveAction = dispatch(saveOmbiClient({
            ombi: ombi,
          }));
        }
        else if (client === "Overseerr") {
          saveAction = dispatch(saveOverseerrClient({
            overseerr: overseerr,
          }));
        }
        else if (client === "Radarr") {
          saveAction = dispatch(saveRadarrClient({
            radarr: radarr,
          }));
        }

        saveAction.then(data => {
          setIsSaving(false);
          setIsSubmitted(false);

          if (data.ok) {
            setSaveAttempted(true);
            setSaveError("");
            setSaveSuccess(true);
          } else {
            var error = "An unknown error occurred while saving.";

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
        setIsSubmitted(false);
      }
    }
  }, [isSubmitted]);




  // const validateNonEmptyString = value => {
  //   return /\S/.test(value);
  // }


  const onClientChange = (newClient) => {
    setClient(newClient);
    setRadarr(reduxState.settings.radarr);
    setOmbi(reduxState.settings.ombi);
    setOverseerr(reduxState.settings.overseerr);
    setSaveAttempted(false);
    setIsSubmitted(false);
  }

  const onSaving = (e) => {
    e.preventDefault();
    setIsSubmitted(true);
  }

  const renderClientConfig = () => {
    if (client === "Disabled") return null;
    
    if (client === "Radarr") {
      return (
        <Radarr 
          onChange={newRadarr => setRadarr(newRadarr)} 
          onValidate={newIsRadarrValid => setIsRadarrValid(newIsRadarrValid)} 
          isSubmitted={isSubmitted} 
          isSaving={isSaving} 
        />
      );
    }
    
    if (client === "Ombi") {
      return (
        <Ombi 
          type={"movie"} 
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
        title="Movies" 
        description="Configure connection between your bot and movie download client"
        icon="fas fa-film"
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
                      Select a download client for movie requests
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
                          description="No movie download client"
                          icon="fas fa-ban"
                          isActive={client === "Disabled"}
                          onClick={() => onClientChange("Disabled")}
                          color="secondary"
                        />
                      </Col>
                      <Col lg="3">
                        <ClientCard
                          title="Radarr"
                          description="Connect to Radarr for movie management"
                          icon="fas fa-film"
                          isActive={client === "Radarr"}
                          onClick={() => onClientChange("Radarr")}
                          color="primary"
                        />
                      </Col>
                      <Col lg="3">
                        <ClientCard
                          title="Overseerr"
                          description="Connect to Overseerr for movie requests"
                          icon="fas fa-server"
                          isActive={client === "Overseerr"}
                          onClick={() => onClientChange("Overseerr")}
                          color="info"
                        />
                      </Col>
                      <Col lg="3">
                        <ClientCard
                          title="Ombi"
                          description="Connect to Ombi for movie requests"
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
                              <strong>Changing the download client will delete all pending movie notifications.</strong>
                            </Alert>
                      </Col>
                    </Row>
                    )}
                    
                    {client !== "Disabled" && (
                      <>
                        <hr className="my-4" />
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

export default Movies;