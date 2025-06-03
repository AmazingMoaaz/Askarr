import React from "react";
import PropTypes from "prop-types";
import { Card, CardBody, CardTitle, CardText, Button } from "reactstrap";

const ActionCard = ({
  title,
  description,
  icon,
  iconColor = "primary",
  actionText = "View Details",
  onClick,
  disabled = false,
  loading = false,
  className = ""
}) => {
  return (
    <Card className={`card-hover-reveal ${className}`}>
      <CardBody className="text-center p-4">
        <div className={`card-icon icon-${iconColor} mx-auto mb-4`}>
          <i className={icon}></i>
        </div>
        <CardTitle tag="h4">{title}</CardTitle>
        <CardText>{description}</CardText>
        <Button
          color={iconColor}
          onClick={onClick}
          disabled={disabled || loading}
          className={loading ? "btn-loading mt-3" : "mt-3"}
        >
          {actionText}
        </Button>
      </CardBody>
      <div className="card-hover-content">
        <h4 className="text-white mb-4">{title}</h4>
        <p className="text-white mb-4">{description}</p>
        <Button
          color="light"
          onClick={onClick}
          disabled={disabled || loading}
          className={loading ? "btn-loading" : ""}
        >
          {actionText}
        </Button>
      </div>
    </Card>
  );
};

ActionCard.propTypes = {
  title: PropTypes.string.isRequired,
  description: PropTypes.string.isRequired,
  icon: PropTypes.string.isRequired,
  iconColor: PropTypes.oneOf([
    "primary",
    "success",
    "warning",
    "danger",
    "info"
  ]),
  actionText: PropTypes.string,
  onClick: PropTypes.func.isRequired,
  disabled: PropTypes.bool,
  loading: PropTypes.bool,
  className: PropTypes.string
};

export default ActionCard; 