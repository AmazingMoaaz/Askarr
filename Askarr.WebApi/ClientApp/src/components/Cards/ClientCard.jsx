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
  return (
    <Card 
      className={`modern-card client-card mb-4 ${isActive ? 'active' : ''}`} 
      onClick={onClick}
      style={{ cursor: 'pointer' }}
    >
      <CardBody>
        <div className="d-flex align-items-center">
          <div 
            className={`icon icon-shape icon-lg bg-${color} text-white rounded-circle shadow mr-4`}
            style={{ width: '60px', height: '60px', display: 'flex', alignItems: 'center', justifyContent: 'center' }}
          >
            <i className={icon} style={{ fontSize: '1.5rem' }}></i>
          </div>
          <div>
            <div className="d-flex align-items-center mb-1">
              <h4 className="mb-0 mr-2">{title}</h4>
              {isActive && (
                <Badge color="success" pill className="ml-2">
                  Active
                </Badge>
              )}
            </div>
            <p className="text-sm text-muted mb-0">{description}</p>
          </div>
        </div>
      </CardBody>
    </Card>
  );
};

export default ClientCard; 