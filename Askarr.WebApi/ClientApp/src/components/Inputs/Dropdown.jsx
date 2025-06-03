import { useState, useEffect } from "react";
import { Select } from "react-dropdown-select";

import {
  FormGroup,
} from "reactstrap";


function Dropdown(props) {
  const [selectedValues, setSelectedValues] = useState([]);

  // Add null/undefined checks for props.items
  const items = props.items || [];
  
  useEffect(() => {
    // Update selectedValues when props.value changes
    if (props.value && items.length > 0) {
      const newSelectedValue = items.find(item => item.value === props.value);
      if (newSelectedValue) {
        setSelectedValues([newSelectedValue]);
      } else if (items.length > 0) {
        setSelectedValues([items[0]]);
      }
    } else if (items.length > 0) {
      setSelectedValues([items[0]]);
    } else {
      setSelectedValues([]);
    }
  }, [props.value, items]);

  const onValueChange = (value) => {
    if (value && value.length > 0) {
      props.onChange(value[0].value);
    }
  };

  return (
    <FormGroup className={props.className}>
      {props.name && (
        <label className="form-control-label">
          {props.name}
        </label>
      )}

      <Select
        placeholder={props.placeholder || ""}
        className="dropdown"
        options={items}
        values={selectedValues}
        labelField="name"
        valueField="value"
        searchable={props.searchable || false}
        clearable={false}
        onChange={onValueChange}
      />
    </FormGroup>
  );
}

export default Dropdown;