import React from "react";
import PropTypes from "prop-types";
import { Card, CardBody, Row, Col } from "reactstrap";

const StatsCard = ({
  title,
  value,
  icon,
  iconColor = "primary",
  change = null,
  changeType = "neutral",
  changeText = "",
  footer = null,
  className = ""
}) => {
  // Helper to get gradient for icon
  const getIconGradient = (color) => {
    const gradients = {
      primary: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
      success: 'linear-gradient(135deg, #2dce89 0%, #2dcecc 100%)',
      warning: 'linear-gradient(135deg, #fb6340 0%, #fbb140 100%)',
      danger: 'linear-gradient(135deg, #f5365c 0%, #f56036 100%)',
      info: 'linear-gradient(135deg, #11cdef 0%, #1171ef 100%)',
    };
    return gradients[color] || gradients.primary;
  };
  
  // Helper to determine change indicator class
  const getChangeColor = (type) => {
    switch (type) {
      case "positive":
        return "#2dce89";
      case "negative":
        return "#f5365c";
      default:
        return "#8898aa";
    }
  };
  
  // Helper to determine change icon
  const getChangeIcon = (type) => {
    switch (type) {
      case "positive":
        return "fa-arrow-up";
      case "negative":
        return "fa-arrow-down";
      default:
        return "fa-minus";
    }
  };
  
  return (
    <Card className={`stats-card modern-card ${className}`}>
      <CardBody className="p-4">
        <Row className="align-items-center">
          <Col xs="auto">
            <div 
              className="stats-icon"
              style={{
                background: getIconGradient(iconColor),
              }}
            >
              <i className={icon}></i>
            </div>
          </Col>
          <Col>
            <div>
              <p 
                className="stats-title mb-2"
                style={{ 
                  fontSize: '0.875rem',
                  fontWeight: '700',
                  color: '#8898aa',
                  textTransform: 'uppercase',
                  letterSpacing: '0.05em'
                }}
              >
                {title}
              </p>
              <h2 
                className="stats-value mb-0"
                style={{
                  fontSize: '2.25rem',
                  fontWeight: '800',
                  background: getIconGradient(iconColor),
                  WebkitBackgroundClip: 'text',
                  WebkitTextFillColor: 'transparent',
                  backgroundClip: 'text',
                }}
              >
                {value}
              </h2>
              {change !== null && (
                <div 
                  className="mt-2"
                  style={{
                    fontSize: '0.875rem',
                    fontWeight: '600',
                    color: getChangeColor(changeType),
                  }}
                >
                  <i className={`fas ${getChangeIcon(changeType)} mr-1`}></i>
                  <span>
                    {change}
                    {changeText && ` ${changeText}`}
                  </span>
                </div>
              )}
            </div>
          </Col>
        </Row>
        {footer && (
          <div 
            className="mt-3 pt-3"
            style={{ 
              borderTop: '1px solid rgba(0, 0, 0, 0.05)',
              fontSize: '0.875rem',
              color: '#8898aa'
            }}
          >
            {footer}
          </div>
        )}
      </CardBody>
    </Card>
  );
};

StatsCard.propTypes = {
  title: PropTypes.string.isRequired,
  value: PropTypes.oneOfType([
    PropTypes.string,
    PropTypes.number,
    PropTypes.node
  ]).isRequired,
  icon: PropTypes.string.isRequired,
  iconColor: PropTypes.oneOf([
    "primary",
    "success",
    "warning",
    "danger",
    "info"
  ]),
  change: PropTypes.oneOfType([
    PropTypes.string,
    PropTypes.number
  ]),
  changeType: PropTypes.oneOf([
    "positive",
    "negative",
    "neutral"
  ]),
  changeText: PropTypes.string,
  footer: PropTypes.node,
  className: PropTypes.string
};

export default StatsCard; 