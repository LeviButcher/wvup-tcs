import React, { useState } from 'react';
import { CSVLink } from 'react-csv';
import StartToEndDateForm from '../StartToEndDateForm';
import { Link, Table, Header, ReportLayout, Button, Card } from '../../ui';
import { Gear, Trashcan } from '../../ui/icons';
import unwrapToJSON from '../../utils/unwrapToJSON';
import ensureResponseCode from '../../utils/ensureResponseCode';

import callApi from '../../utils/callApi';

const getClassTours = (startDate, endDate) =>
  callApi(`classtours/?start=${startDate}&end=${endDate}`, 'GET', null);

const deleteTour = id => callApi(`classtours/${id}`, 'DELETE', null);

const ClassTourLookup = () => {
  const [tours, setTours] = useState();
  const [start, setStart] = useState();
  const [end, setEnd] = useState();

  const loadTours = (startDate, endDate) =>
    getClassTours(startDate, endDate)
      .then(ensureResponseCode(200))
      .then(unwrapToJSON)
      .then(setTours);

  return (
    <ReportLayout>
      <div>
        <Card>
          <Header type="h3">Additional Actions</Header>
          <Link to="create">
            <Button intent="secondary" align="left">
              Add Class Tour
            </Button>
          </Link>
        </Card>
        <StartToEndDateForm
          name="ClassTour Lookup"
          initialValues={{ startDate: '', endDate: '' }}
          onSubmit={(values, { setSubmitting, setStatus }) => {
            const { startDate, endDate } = values;
            loadTours(startDate, endDate)
              .catch(e => {
                setStatus({ msg: e.message });
              })
              .finally(() => {
                setStart(startDate);
                setEnd(endDate);
                setSubmitting(false);
              });
          }}
        />
      </div>
      {tours && (
        <Card width="900px">
          <Table>
            <caption>
              <Header>
                Class Tours -{' '}
                <CSVLink data={tours} filename="classTourLookup">
                  Download
                </CSVLink>
              </Header>
            </caption>
            <thead align="left">
              <tr>
                <th>Name</th>
                <th align="center">Total Tourists</th>
                <th align="center">Date</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody align="left">
              {tours.map(tour => (
                <ClassTourRow
                  key={tour.id}
                  tour={tour}
                  afterDelete={() => loadTours(start, end)}
                />
              ))}
            </tbody>
          </Table>
        </Card>
      )}
    </ReportLayout>
  );
};

const ClassTourRow = ({
  tour: { id, name, numberOfStudents, dayVisited },
  afterDelete
}) => (
  <tr>
    <td>{name}</td>
    <td align="center">{numberOfStudents}</td>
    <td align="center">{new Date(dayVisited).toLocaleString()}</td>
    <td style={{ display: 'flex', justifyContent: 'space-evenly' }}>
      <Link to={`update/${id}`}>
        <Gear />
      </Link>
      <Trashcan
        onClick={() => {
          const goDelete = window.confirm(
            `You sure you want to delete ${name} record`
          );
          if (goDelete) deleteTour(id).then(afterDelete);
        }}
      />
    </td>
  </tr>
);

export default ClassTourLookup;
