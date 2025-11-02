import React from "react";
import { Card, CardBody, Badge } from "reactstrap";

const ClientCard = ({ 
  title, 
  description, 
  icon, 
  isActive, 
  onClick, 
  color = "primary" 
}) => {
  const gradientMap = {
    primary: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
    info: 'linear-gradient(135deg, #11cdef 0%, #1171ef 100%)',
    success: 'linear-gradient(135deg, #2dce89 0%, #2dcecc 100%)',
    danger: 'linear-gradient(135deg, #f5365c 0%, #f56036 100%)',
    warning: 'linear-gradient(135deg, #fb6340 0%, #fbb140 100%)',
  };

  return (
    <Card 
      className={`modern-card client-card mb-4 ${isActive ? 'active' : ''}`} 
      onClick={onClick}
      style={{ cursor: 'pointer' }}
    >
      <CardBody className="p-4">
        <div className="d-flex align-items-center">
          <div 
            className="icon-shape mr-4"
            style={{ 
              width: '75px', 
              height: '75px', 
              display: 'flex', 
              alignItems: 'center', 
              justifyContent: 'center',
              borderRadius: '1.25rem',
              background: gradientMap[color] || gradientMap.primary,
            }}
          >
            <i className={icon} style={{ fontSize: '2rem', color: '#fff' }}></i>
          </div>
          <div className="flex-grow-1">
            <div className="d-flex align-items-center justify-content-between mb-2">
              <h3 
                className="mb-0 font-weight-bold" 
                style={{ 
                  fontSize: '1.5rem',
                  background: 'linear-gradient(135deg, #32325d, #667eea)',
                  WebkitBackgroundClip: 'text',
                  WebkitTextFillColor: 'transparent',
                  backgroundClip: 'text',
                }}
              >
                {title}
              </h3>
              {isActive && (
                <Badge 
                  color="success" 
                  pill 
                  className="px-3 py-2"
                  style={{
                    fontSize: '0.75rem',
                    fontWeight: '700',
                    letterSpacing: '0.05em',
                    background: 'linear-gradient(135deg, #2dce89, #2dcecc)',
                    boxShadow: '0 5px 15px rgba(45, 206, 137, 0.4)',
                  }}
                >
                  <i className="fas fa-check-circle mr-1"></i>
                  ACTIVE
                </Badge>
              )}
            </div>
            <p 
              className="mb-0" 
              style={{ 
                color: '#8898aa', 
                fontSize: '0.95rem',
                lineHeight: '1.6'
              }}
            >
              {description}
            </p>
          </div>
        </div>
      </CardBody>
    </Card>
  );
};

export default ClientCard; 