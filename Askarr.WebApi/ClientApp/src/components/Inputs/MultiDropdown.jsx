import { Select } from "react-dropdown-select";

import {
  FormGroup,
} from "reactstrap";


function MultiDropdown(props) {
  // Add null/undefined checks for props
  const items = props.items || [];
  const selectedItems = props.selectedItems || [];

  const onSelectedItemsChange = (selectedItems) => {
    props.onChange(selectedItems);
  }

  // Calculate filtered values with proper null checks
  const getFilteredValues = () => {
    if (items.length === 0) return [];
    
    const filtered = items.filter(x => 
      selectedItems.length > 0 && 
      selectedItems.map(s => s.id).includes(x.id)
    );
    
    return filtered.length > 0 ? filtered : [];
  };

  return (
    <FormGroup className={props.className}>
      <label
        className="form-control-label"
        style={{ 
          fontWeight: '600',
          color: '#2D3748',
          fontSize: '0.875rem',
          marginBottom: '0.5rem',
          display: 'block'
        }}>
        {props.name}
      </label>
      <Select
        multi="true"
        className={props.create === true ? "dropdown react-dropdown-create" : "dropdown"}
        options={items}
        placeholder={props.placeholder || "Select options..."}
        values={getFilteredValues()}
        labelField="name"
        valueField="id"
        dropdownHandle={props.dropdownHandle !== false}
        searchable={props.searchable === true}
        create={props.create === true}
        clearable={false}
        color="#4FD1C5"
        onChange={(values) => onSelectedItemsChange(values)}
      />
    </FormGroup>
  );
}

export default MultiDropdown;