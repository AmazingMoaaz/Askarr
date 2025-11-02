import React from "react";
import { Container, Row, Col } from "reactstrap";

const ModernHeader = ({ title, description, icon }) => {
  return (
    <div className="modern-header">
      <Container fluid>
        <div className="header-body" style={{ position: 'relative', zIndex: 2 }}>
          <Row className="align-items-center py-4">
            <Col>
              <div className="d-flex align-items-center">
                {icon && (
                  <div 
                    className="icon icon-shape icon-lg rounded-circle shadow-lg mr-4"
                    style={{ 
                      width: '80px', 
                      height: '80px',
                      background: 'linear-gradient(135deg, rgba(255, 255, 255, 0.2), rgba(255, 255, 255, 0.1))',
                      backdropFilter: 'blur(10px)',
                      border: '2px solid rgba(255, 255, 255, 0.3)',
                      display: 'flex',
                      alignItems: 'center',
                      justifyContent: 'center',
                      boxShadow: '0 15px 40px rgba(0, 0, 0, 0.3)',
                      transition: 'all 0.4s cubic-bezier(0.4, 0, 0.2, 1)',
                    }}
                    onMouseEnter={(e) => {
                      e.currentTarget.style.transform = 'scale(1.1) rotate(10deg)';
                      e.currentTarget.style.boxShadow = '0 20px 60px rgba(0, 0, 0, 0.4)';
                    }}
                    onMouseLeave={(e) => {
                      e.currentTarget.style.transform = 'scale(1) rotate(0deg)';
                      e.currentTarget.style.boxShadow = '0 15px 40px rgba(0, 0, 0, 0.3)';
                    }}
                  >
                    <i className={icon} style={{ fontSize: '2.5rem', color: '#fff' }}></i>
                  </div>
                )}
                <div>
                  <h1 className="header-title mb-2" style={{ 
                    letterSpacing: '0.05em',
                    animation: 'fadeInUp 0.8s ease-out'
                  }}>
                    {title}
                  </h1>
                  {description && (
                    <p className="header-subtitle mb-0" style={{
                      animation: 'fadeInUp 0.8s ease-out 0.2s both'
                    }}>
                      {description}
                    </p>
                  )}
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