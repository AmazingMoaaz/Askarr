import React from "react";
import PropTypes from "prop-types";
import { FormGroup, Input, FormFeedback } from "reactstrap";

const FloatingLabelInput = ({
  id,
  name,
  type = "text",
  label,
  value,
  onChange,
  placeholder = " ",
  required = false,
  disabled = false,
  readOnly = false,
  invalid = false,
  valid = false,
  feedback = "",
  className = "",
  ...rest
}) => {
  return (
    <FormGroup className={`has-floating-label ${className}`}>
      <Input
        id={id}
        name={name}
        type={type}
        value={value}
        onChange={onChange}
        placeholder={placeholder}
        required={required}
        disabled={disabled}
        readOnly={readOnly}
        invalid={invalid}
        valid={valid}
        className="form-control"
        {...rest}
      />
      <label htmlFor={id} className="floating-label">
        {label}
        {required && <span className="text-danger ml-1">*</span>}
      </label>
      {invalid && feedback && <FormFeedback>{feedback}</FormFeedback>}
    </FormGroup>
  );
};

FloatingLabelInput.propTypes = {
  id: PropTypes.string.isRequired,
  name: PropTypes.string.isRequired,
  type: PropTypes.string,
  label: PropTypes.string.isRequired,
  value: PropTypes.oneOfType([
    PropTypes.string,
    PropTypes.number,
    PropTypes.bool
  ]),
  onChange: PropTypes.func.isRequired,
  placeholder: PropTypes.string,
  required: PropTypes.bool,
  disabled: PropTypes.bool,
  readOnly: PropTypes.bool,
  invalid: PropTypes.bool,
  valid: PropTypes.bool,
  feedback: PropTypes.string,
  className: PropTypes.string
};

export default FloatingLabelInput; 