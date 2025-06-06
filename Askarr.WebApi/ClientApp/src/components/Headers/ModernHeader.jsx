import React from "react";
import { Container, Row, Col } from "reactstrap";

const ModernHeader = ({ title, description, icon }) => {
  return (
    <div className="modern-header">
      <Container fluid>
        <div className="header-body">
          <Row className="align-items-center py-4">
            <Col>
              <div className="d-flex align-items-center">
                {icon && (
                  <div className="icon icon-shape icon-lg bg-white text-primary rounded-circle shadow mr-4">
                    <i className={icon}></i>
                  </div>
                )}
                <div>
                  <h1 className="header-title mb-1">{title}</h1>
                  {description && <p className="header-subtitle mb-0">{description}</p>}
                </div>
              </div>
            </Col>
          </Row>
        </div>
      </Container>
    </div>
  );
};

export default ModernHeader; 