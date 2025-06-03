import React, { useState, useEffect } from "react";
import PropTypes from "prop-types";
import {
  Table,
  Pagination,
  PaginationItem,
  PaginationLink,
  Input,
  Row,
  Col
} from "reactstrap";

const SortableTable = ({
  columns,
  data,
  initialSortField = null,
  initialSortDirection = "asc",
  onRowClick = null,
  emptyMessage = "No data available",
  pageSize = 10,
  searchable = false,
  className = "",
  responsive = true
}) => {
  // State for sorting
  const [sortField, setSortField] = useState(initialSortField);
  const [sortDirection, setSortDirection] = useState(initialSortDirection);
  
  // State for pagination
  const [currentPage, setCurrentPage] = useState(1);
  
  // State for search
  const [searchTerm, setSearchTerm] = useState("");
  const [filteredData, setFilteredData] = useState([]);
  
  // Effect to filter and sort data
  useEffect(() => {
    let result = [...data];
    
    // Apply search filter if searchable
    if (searchable && searchTerm) {
      result = result.filter(item => {
        return columns.some(column => {
          const value = column.selector(item);
          return value && value.toString().toLowerCase().includes(searchTerm.toLowerCase());
        });
      });
    }
    
    // Apply sorting if sortField is provided
    if (sortField) {
      const sortColumn = columns.find(col => col.field === sortField);
      if (sortColumn) {
        result.sort((a, b) => {
          const aValue = sortColumn.selector(a);
          const bValue = sortColumn.selector(b);
          
          // Handle null or undefined values
          if (aValue === null || aValue === undefined) return sortDirection === "asc" ? -1 : 1;
          if (bValue === null || bValue === undefined) return sortDirection === "asc" ? 1 : -1;
          
          // Handle different data types
          if (typeof aValue === "string") {
            return sortDirection === "asc" 
              ? aValue.localeCompare(bValue) 
              : bValue.localeCompare(aValue);
          } else {
            return sortDirection === "asc" 
              ? aValue - bValue 
              : bValue - aValue;
          }
        });
      }
    }
    
    setFilteredData(result);
    setCurrentPage(1); // Reset to first page when data changes
  }, [data, searchTerm, sortField, sortDirection, columns]);
  
  // Handle sort toggle
  const handleSort = (field) => {
    if (sortField === field) {
      // Toggle direction if same field
      setSortDirection(sortDirection === "asc" ? "desc" : "asc");
    } else {
      // Set new field and default to ascending
      setSortField(field);
      setSortDirection("asc");
    }
  };
  
  // Calculate pagination
  const totalPages = Math.ceil(filteredData.length / pageSize);
  const startIndex = (currentPage - 1) * pageSize;
  const endIndex = Math.min(startIndex + pageSize, filteredData.length);
  const currentData = filteredData.slice(startIndex, endIndex);
  
  // Handle page change
  const handlePageChange = (page) => {
    setCurrentPage(page);
  };
  
  // Render pagination controls
  const renderPagination = () => {
    if (totalPages <= 1) return null;
    
    const pages = [];
    const maxVisiblePages = 5;
    
    // Calculate range of visible pages
    let startPage = Math.max(1, currentPage - Math.floor(maxVisiblePages / 2));
    let endPage = Math.min(totalPages, startPage + maxVisiblePages - 1);
    
    // Adjust if we're near the end
    if (endPage - startPage + 1 < maxVisiblePages) {
      startPage = Math.max(1, endPage - maxVisiblePages + 1);
    }
    
    // Previous button
    pages.push(
      <PaginationItem key="prev" disabled={currentPage === 1}>
        <PaginationLink previous onClick={() => handlePageChange(currentPage - 1)} />
      </PaginationItem>
    );
    
    // First page
    if (startPage > 1) {
      pages.push(
        <PaginationItem key="1">
          <PaginationLink onClick={() => handlePageChange(1)}>1</PaginationLink>
        </PaginationItem>
      );
      
      // Ellipsis if needed
      if (startPage > 2) {
        pages.push(
          <PaginationItem key="ellipsis1" disabled>
            <PaginationLink>...</PaginationLink>
          </PaginationItem>
        );
      }
    }
    
    // Page numbers
    for (let i = startPage; i <= endPage; i++) {
      pages.push(
        <PaginationItem key={i} active={i === currentPage}>
          <PaginationLink onClick={() => handlePageChange(i)}>{i}</PaginationLink>
        </PaginationItem>
      );
    }
    
    // Last page
    if (endPage < totalPages) {
      // Ellipsis if needed
      if (endPage < totalPages - 1) {
        pages.push(
          <PaginationItem key="ellipsis2" disabled>
            <PaginationLink>...</PaginationLink>
          </PaginationItem>
        );
      }
      
      pages.push(
        <PaginationItem key={totalPages}>
          <PaginationLink onClick={() => handlePageChange(totalPages)}>
            {totalPages}
          </PaginationLink>
        </PaginationItem>
      );
    }
    
    // Next button
    pages.push(
      <PaginationItem key="next" disabled={currentPage === totalPages}>
        <PaginationLink next onClick={() => handlePageChange(currentPage + 1)} />
      </PaginationItem>
    );
    
    return (
      <Pagination className="pagination justify-content-center">
        {pages}
      </Pagination>
    );
  };
  
  // Render empty state
  const renderEmptyState = () => (
    <tr>
      <td colSpan={columns.length} className="text-center py-5">
        <div className="empty-state">
          <div className="empty-icon">
            <i className="fas fa-database"></i>
          </div>
          <div className="empty-text">{emptyMessage}</div>
        </div>
      </td>
    </tr>
  );
  
  return (
    <div className={`sortable-table ${responsive ? 'table-responsive' : ''}`}>
      {searchable && (
        <Row className="mb-3">
          <Col md="4" className="ml-auto">
            <Input
              type="search"
              placeholder="Search..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="form-control-alternative"
            />
          </Col>
        </Row>
      )}
      
      <Table className={`${className}`} hover>
        <thead className="sortable-header">
          <tr>
            {columns.map((column) => (
              <th
                key={column.field}
                className={`${column.sortable !== false ? 'sortable' : ''} ${
                  sortField === column.field ? sortDirection : ''
                }`}
                onClick={() => column.sortable !== false && handleSort(column.field)}
                style={{ width: column.width || 'auto' }}
              >
                {column.name}
                {column.sortable !== false && (
                  <span className="sort-icon"></span>
                )}
              </th>
            ))}
          </tr>
        </thead>
        <tbody>
          {currentData.length > 0 ? (
            currentData.map((row, index) => (
              <tr
                key={index}
                onClick={() => onRowClick && onRowClick(row)}
                style={{ cursor: onRowClick ? "pointer" : "default" }}
              >
                {columns.map((column) => (
                  <td key={column.field} data-label={column.name}>
                    {column.cell ? column.cell(row) : column.selector(row)}
                  </td>
                ))}
              </tr>
            ))
          ) : (
            renderEmptyState()
          )}
        </tbody>
      </Table>
      
      {filteredData.length > 0 && (
        <div className="table-footer">
          <div className="pagination-info">
            Showing {startIndex + 1} to {endIndex} of {filteredData.length} entries
          </div>
          {renderPagination()}
        </div>
      )}
    </div>
  );
};

SortableTable.propTypes = {
  columns: PropTypes.arrayOf(
    PropTypes.shape({
      name: PropTypes.string.isRequired,
      field: PropTypes.string.isRequired,
      selector: PropTypes.func.isRequired,
      sortable: PropTypes.bool,
      cell: PropTypes.func,
      width: PropTypes.string
    })
  ).isRequired,
  data: PropTypes.array.isRequired,
  initialSortField: PropTypes.string,
  initialSortDirection: PropTypes.oneOf(["asc", "desc"]),
  onRowClick: PropTypes.func,
  emptyMessage: PropTypes.string,
  pageSize: PropTypes.number,
  searchable: PropTypes.bool,
  className: PropTypes.string,
  responsive: PropTypes.bool
};

export default SortableTable; 