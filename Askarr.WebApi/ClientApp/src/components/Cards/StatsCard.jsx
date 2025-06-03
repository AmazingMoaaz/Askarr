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
  // Helper to determine icon background color
  const getIconClass = (color) => {
    return `icon-${color}`;
  };
  
  // Helper to determine change indicator class
  const getChangeClass = (type) => {
    switch (type) {
      case "positive":
        return "positive";
      case "negative":
        return "negative";
      default:
        return "";
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
    <Card className={`card-stats ${className}`}>
      <CardBody>
        <Row>
          <Col xs="3">
            <div className={`card-icon ${getIconClass(iconColor)}`}>
              <i className={icon}></i>
            </div>
          </Col>
          <Col xs="9">
            <div className="numbers">
              <p className="card-status-title">{title}</p>
              <h3 className="card-status-value">{value}</h3>
              {change !== null && (
                <div className={`card-stats-compare ${getChangeClass(changeType)}`}>
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
          <div className="mt-3 pt-3 border-top">
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