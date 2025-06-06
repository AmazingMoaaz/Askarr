import { useEffect, useState } from "react";
import { useDispatch, useSelector } from 'react-redux';
import { Alert } from "reactstrap";
import { getSettings } from "../store/actions/MusicClientsActions";
import { saveDisabledClient } from "../store/actions/MusicClientsActions";
import { saveLidarrClient } from "../store/actions/LidarrClientActions";
import Lidarr from "../components/DownloadClients/Lidarr/Lidarr";
import ClientCard from "../components/Cards/ClientCard";
import ModernHeader from "../components/Headers/ModernHeader";

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

function Music() {
  const [isSubmitted, setIsSubmitted] = useState(false);
  const [isLoading, setIsLoading] = useState(true);
  const [isSaving, setIsSaving] = useState(false);
  const [saveAttempted, setSaveAttempted] = useState(false);
  const [saveSuccess, setSaveSuccess] = useState(false);
  const [saveError, setSaveError] = useState("");
  const [client, setClient] = useState("");
  const [lidarr, setLidarr] = useState({});
  const [isLidarrValid, setIsLidarrValid] = useState(false);

  const reduxState = useSelector((state) => {
    return {
      settings: state.music
    };
  });
  const dispatch = useDispatch();

  useEffect(() => {
    dispatch(getSettings())
      .then(data => {
        setIsLoading(false);
        setClient(data.payload.client);
        setLidarr(data.payload.lidarr);
      });
  }, []);

  useEffect(() => {
    if (!isSubmitted)
      return;

    if (!isSaving) {
      if ((client === "Disabled" || (client === "Lidarr" && isLidarrValid))) {
        setIsSaving(true);

        let saveAction = null;

        if (client === "Disabled") {
          saveAction = dispatch(saveDisabledClient());
        } else if (client === "Lidarr") {
          saveAction = dispatch(saveLidarrClient({
            lidarr: lidarr,
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

  const onClientChange = (newClient) => {
    setClient(newClient);
    setLidarr(reduxState.settings.lidarr);
    setSaveAttempted(false);
    setIsSubmitted(false);
  }

  const onSaving = (e) => {
    e.preventDefault();
    setIsSubmitted(true);
  }

  const renderClientConfig = () => {
    if (client === "Disabled") return null;
    
    if (client === "Lidarr") {
      return (
        <Lidarr
          onChange={values => setLidarr(values)}
          onValidate={value => setIsLidarrValid(value)}
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
        title="Music" 
        description="Configure connection between your bot and music client"
        icon="fas fa-music"
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
                      Select a download client for music requests
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
                      <Col lg="4" className="mx-auto">
                        <ClientCard
                          title="Disabled"
                          description="No music download client"
                          icon="fas fa-ban"
                          isActive={client === "Disabled"}
                          onClick={() => onClientChange("Disabled")}
                          color="secondary"
                        />
                      </Col>
                      <Col lg="4" className="mx-auto">
                        <ClientCard
                          title="Lidarr"
                          description="Connect to Lidarr for music management"
                          icon="fas fa-music"
                          isActive={client === "Lidarr"}
                          onClick={() => onClientChange("Lidarr")}
                          color="primary"
                        />
                      </Col>
                    </Row>
                    
                    {reduxState.settings.client !== client && reduxState.settings.client !== "Disabled" && (
                      <Row className="mt-4">
                        <Col>
                          <Alert className="text-center" color="warning">
                            <strong>Changing the download client will delete all pending music notifications.</strong>
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

export default Music;