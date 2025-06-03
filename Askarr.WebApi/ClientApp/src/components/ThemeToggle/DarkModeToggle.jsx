import React, { useContext } from "react";
import { ThemeContext } from "../../contexts/ThemeContext";
import { Button } from "reactstrap";

const DarkModeToggle = () => {
  const { darkMode, toggleDarkMode } = useContext(ThemeContext);

  return (
    <Button
      className="nav-link-icon"
      color={darkMode ? "light" : "dark"}
      size="sm"
      onClick={toggleDarkMode}
      title={darkMode ? "Switch to Light Mode" : "Switch to Dark Mode"}
    >
      <i className={`fas fa-${darkMode ? "sun" : "moon"}`} />
      <span className="ml-2 d-none d-lg-inline">
        {darkMode ? "Light Mode" : "Dark Mode"}
      </span>
    </Button>
  );
};

export default DarkModeToggle; 