import * as Yup from 'yup';

const StartToEndDateSchema = Yup.object().shape({
  startDate: Yup.date().required(),
  endDate: Yup.date()
    .test('date-test', 'Must be after the Start Date', function(endDate) {
      return this.resolve(Yup.ref('startDate')) < endDate;
    })
    .required()
});

export default StartToEndDateSchema;
